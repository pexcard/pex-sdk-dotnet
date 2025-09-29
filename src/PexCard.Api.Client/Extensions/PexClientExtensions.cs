using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using PexCard.Api.Client.Const;

namespace PexCard.Api.Client.Extensions
{
    internal static class PexClientExtensions
    {
        private const string PexCorrelationIdHeaderName = "X-CORRELATION-ID";
        private const string PexJsonMediaType = "application/json";
        private const string PexForwardedForHeaderName = "X-Forwarded-For";
        private static readonly Encoding PexEncodingType = Encoding.UTF8;
        private static readonly JsonSerializerSettings PexJsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
        };

        public static void SetXForwardForHeader(this HttpRequestMessage request, string forwardFor)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (!string.IsNullOrEmpty(forwardFor) && !string.IsNullOrWhiteSpace(forwardFor))
            {
                request.Headers.TryAddWithoutValidation(PexForwardedForHeaderName, forwardFor);
            }
        }

        public static void SetPexCorrelationIdHeader(this HttpRequestMessage request, string correlationId)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Headers.TryAddWithoutValidation(PexCorrelationIdHeaderName, correlationId);
        }

        public static string GetPexCorrelationId(this HttpResponseMessage response, bool fallbackToRequestCorrelationId = true)
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            string correlationId = default;

            // ideally external-api responds with a correlation id (i.e. if one was not sent).
            // but, as of 2025-04 it does not yet; so fallback to request correlation id if provided.

            if (response.Headers.TryGetValues(PexCorrelationIdHeaderName, out var responseCorrelationId))
            {
                correlationId = responseCorrelationId.FirstOrDefault();
            }
            else if (fallbackToRequestCorrelationId && response.RequestMessage.Headers.TryGetValues(PexCorrelationIdHeaderName, out var requestCorrelationId))
            {
                correlationId = requestCorrelationId.FirstOrDefault();
            }

            return correlationId;
        }

        public static HttpRequestMessage SetPexAuthorizationBearerHeader(this HttpRequestMessage request, string bearerToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Bearer, bearerToken);

            return request;
        }

        public static HttpRequestMessage SetPexAuthorizationTokenHeader(this HttpRequestMessage request, string externalToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            return request;
        }

        public static HttpRequestMessage SetPexAuthorizationBasicHeader(this HttpRequestMessage request, string appId, string appSecret)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrEmpty(appId))
            {
                throw new ArgumentException($"'{nameof(appId)}' cannot be null or empty.", nameof(appId));
            }

            if (string.IsNullOrEmpty(appSecret))
            {
                throw new ArgumentException($"'{nameof(appSecret)}' cannot be null or empty.", nameof(appSecret));
            }

            var credentials = Convert.ToBase64String(PexEncodingType.GetBytes($"{appId}:{appSecret}"));
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Basic, credentials);

            return request;
        }

        public static HttpRequestMessage SetPexAcceptJsonHeader(this HttpRequestMessage request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(PexJsonMediaType));

            return request;
        }

        public static HttpRequestMessage SetPexJsonContent<TData>(this HttpRequestMessage request, TData contentData)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Content = new StringContent(ToPexJson(contentData), PexEncodingType, PexJsonMediaType);

            return request;
        }

        public static bool IsPexJsonContent(this HttpResponseMessage response)
        {
            return response.Content?.Headers?.IsPexJsonContent() ?? false;
        }

        public static bool IsPexJsonContent(this HttpHeaders headers)
        {
            return headers.TryGetValues("Content-Type", out var contentTypes) && contentTypes.Contains(PexJsonMediaType);
        }

        public static string ToPexJson<TData>(this TData data)
        {
            return JsonConvert.SerializeObject(data, PexJsonSettings);
        }

        public static TData FromPexJson<TData>(this string json)
        {
            return JsonConvert.DeserializeObject<TData>(json, PexJsonSettings);
        }
    }
}
