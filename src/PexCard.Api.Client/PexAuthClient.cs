using Newtonsoft.Json;
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
        private readonly IIPAddressResolver _ipAddressResolver;
        private readonly ICorrelationIdResolver _correlationIdResolver;

        public PexAuthClient(HttpClient httpClient,
                             IIPAddressResolver ipAddressResolver = null,
                             ICorrelationIdResolver correlationIdResolver = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _ipAddressResolver = ipAddressResolver ?? new DummyIpAddressResolver();
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
                request.SetXForwardForHeader(_ipAddressResolver.GetValue());
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
                request.SetXForwardForHeader(_ipAddressResolver.GetValue());
                request.SetPexJsonContent(requestData);

                var response = await _httpClient.SendAsync(request, cancelToken);

                await HandleHttpResponseMessage(response);
            }

            return RevokePartnerAuthTokenAsync();
        }

        public Task<CreateTokenResponseModel> CreateToken(CreateTokenRequestModel request, CancellationToken cancelToken = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (string.IsNullOrEmpty(request.AppId))
            {
                throw new ArgumentException($"'{nameof(request.AppId)}' cannot be null or empty.", nameof(request.AppId));
            }
            if (string.IsNullOrEmpty(request.AppSecret))
            {
                throw new ArgumentException($"'{nameof(request.AppSecret)}' cannot be null or empty.", nameof(request.AppSecret));
            }
            if (string.IsNullOrEmpty(request.Username))
            {
                throw new ArgumentException($"'{nameof(request.Username)}' cannot be null or empty.", nameof(request.Username));
            }
            if (string.IsNullOrEmpty(request.Password))
            {
                throw new ArgumentException($"'{nameof(request.Password)}' cannot be null or empty.", nameof(request.Password));
            }
            if (string.IsNullOrEmpty(request.UserAgentString))
            {
                throw new ArgumentException($"'{nameof(request.UserAgentString)}' cannot be null or empty.", nameof(request.UserAgentString));
            }

            async Task<CreateTokenResponseModel> CreateTokenAsync()
            {
                var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "/token"));

                var requestData = new AuthTokenRequestModel
                {
                    Username = request.Username,
                    Password = request.Password,
                    UserAgentString = request.UserAgentString
                };

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
                httpRequest.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
                httpRequest.SetXForwardForHeader(_ipAddressResolver.GetValue());
                httpRequest.SetPexAuthorizationBasicHeader(request.AppId, request.AppSecret);
                httpRequest.SetPexAcceptJsonHeader();
                httpRequest.SetPexJsonContent(requestData);

                var response = await _httpClient.SendAsync(httpRequest, cancelToken);

                var responseModel = await HandleHttpResponseMessage<AuthTokenResponseModel>(response);
                return new CreateTokenResponseModel
                {
                    Token = responseModel?.Token
                };
            }

            return CreateTokenAsync();
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

                    var correlationId = response.GetPexCorrelationId();

                    if (response.IsPexJsonContent())
                    {
                        var errorModel = JsonConvert.DeserializeObject<ErrorMessageModel>(responseData);

                        throw new PexAuthClientException(response.StatusCode, errorModel?.Message ?? response.ReasonPhrase, correlationId);
                    }
                    else
                    {
                        throw new PexAuthClientException(response.StatusCode, response.ReasonPhrase ?? $"Error {response.StatusCode}", correlationId);
                    }
                }
                return responseData.FromPexJson<TData>();
            }
            catch (PexAuthClientException)
            {
                throw;
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
                    var correlationId = response.GetPexCorrelationId();

                    if (response.IsPexJsonContent())
                    {
                        var errorModel = JsonConvert.DeserializeObject<ErrorMessageModel>(responseData);

                        throw new PexAuthClientException(response.StatusCode, errorModel?.Message ?? response.ReasonPhrase, correlationId);
                    }
                    else
                    {
                        throw new PexAuthClientException(response.StatusCode, response.ReasonPhrase ?? $"Error {response.StatusCode}", correlationId);
                    }
                }
                catch (PexAuthClientException)
                {
                    throw;
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
