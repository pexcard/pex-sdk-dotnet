using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PexCard.Api.Client.Configure;
using PexCard.Api.Client.Core;
using PexCard.Api.Client.Core.Exceptions;
using PexCard.Api.Client.Core.Interfaces;
using PexCard.Api.Client.Core.Models;
using PexCard.Api.Client.Extensions;
using PexCard.Api.Client.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PexCard.Api.Client
{
    public class PexAuthClient : IPexAuthClient
    {
        private readonly HttpClient _httpClient;
        private readonly IIPAddressResolver _ipAddress;
        private readonly ICorrelationIdResolver _correlationIdResolver;

        public PexAuthClient(
            HttpClient httpClient, 
            IIPAddressResolver ipAddress,
            ICorrelationIdResolver correlationIdResolver = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _ipAddress = ipAddress;
            _correlationIdResolver = correlationIdResolver ?? new DefaultCorrelationIdResolver();
        }

        public Uri BaseUri => _httpClient.BaseAddress;

        public Task<Uri> GetPartnerAuthUriAsync(string appId, string appSecret, Uri serverCallbackUri, Uri browserClosingUri, CancellationToken cancelToken = default)
        {
            if (string.IsNullOrEmpty(appId))
            {
                throw new ArgumentException($"'{nameof(appId)}' cannot be null or empty.", nameof(appId));
            }
            if (string.IsNullOrEmpty(appSecret))
            {
                throw new ArgumentException($"'{nameof(appSecret)}' cannot be null or empty.", nameof(appSecret));
            }
            if (serverCallbackUri is null)
            {
                throw new ArgumentNullException(nameof(serverCallbackUri));
            }
            if (browserClosingUri is null)
            {
                throw new ArgumentNullException(nameof(browserClosingUri));
            }

            async Task<Uri> GetPartnerAuthUrlAsync()
            {
                var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "/Auth/Partner"));

                var requestData = new AuthPartnerRequestModel(appId, appSecret, serverCallbackUri, browserClosingUri);

                var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
                request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
                request.SetXForwardFor(_ipAddress.GetValue());
                request.SetPexJsonContent(requestData);

                var response = await _httpClient.SendAsync(request, cancelToken);

                return (await HandleHttpResponseMessage<AuthPartnerResponseModel>(response))?.OauthUrl;
            }

            return GetPartnerAuthUrlAsync();
        }

        public Task RevokePartnerAuthTokenAsync(string appId, string appSecret, string token, CancellationToken cancelToken = default)
        {
            if (string.IsNullOrEmpty(appId))
            {
                throw new ArgumentException($"'{nameof(appId)}' cannot be null or empty.", nameof(appId));
            }
            if (string.IsNullOrEmpty(appSecret))
            {
                throw new ArgumentException($"'{nameof(appSecret)}' cannot be null or empty.", nameof(appSecret));
            }
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException($"'{nameof(token)}' cannot be null or empty.", nameof(token));
            }

            async Task RevokePartnerAuthTokenAsync()
            {
                var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "/Auth/Token"));

                var requestData = new AuthRevokeTokenRequestModel(appId, appSecret, token);

                var request = new HttpRequestMessage(HttpMethod.Delete, requestUriBuilder.Uri);
                request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
                request.SetPexJsonContent(requestData);
                request.SetXForwardFor(_ipAddress.GetValue());

                var response = await _httpClient.SendAsync(request, cancelToken);

                await HandleHttpResponseMessage(response);
            }

            return RevokePartnerAuthTokenAsync();
        }

        #region Private methods

        private async Task<TData> HandleHttpResponseMessage<TData>(HttpResponseMessage response, bool notFoundAsDefault = false)
        {
            var responseData = await response.Content.ReadAsStringAsync();

            try
            {
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NotFound && notFoundAsDefault)
                    {
                        return default;
                    }

                    var errorModel = JsonConvert.DeserializeObject<ErrorMessageModel>(responseData);
                    var correlationId = response.GetPexCorrelationId();

                    throw new PexAuthClientException(response.StatusCode, errorModel.Message, correlationId);
                }
                return responseData.FromPexJson<TData>();
            }
            catch (Exception ex)
            {
                var correlationId = response.GetPexCorrelationId();

                throw new PexAuthClientException(response.StatusCode, $"Error parsing response: {ex.Message}\nContent: {responseData}", ex, correlationId);
            }
        }

        private async Task HandleHttpResponseMessage(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();

                try
                {
                    var errorModel = JsonConvert.DeserializeObject<ErrorMessageModel>(responseData);
                    var correlationId = response.GetPexCorrelationId();

                    throw new PexAuthClientException(response.StatusCode, errorModel.Message, correlationId);
                }
                catch (Exception ex)
                {
                    var correlationId = response.GetPexCorrelationId();

                    throw new PexAuthClientException(response.StatusCode, $"Error parsing response: {ex.Message}\nContent: {responseData}", ex, correlationId);
                }
            }
        }

        #endregion
    }
}
