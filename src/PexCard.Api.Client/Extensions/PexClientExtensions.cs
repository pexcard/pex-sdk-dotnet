using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using PexCard.Api.Client.Const;
using Newtonsoft.Json;
using System.Text;
using System.Diagnostics;

namespace PexCard.Api.Client.Extensions
{
    internal static class PexClientExtensions
    {
        private const string PexCorrelationIdHeaderName = "X-CORRELATION-ID";
        private const string PexJsonMediaType = "application/json";

        private static readonly Encoding PexEncodingType = Encoding.UTF8;

        private static readonly JsonSerializerSettings PexJsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
        };

        public static string GetPexCorrelationId(this HttpResponseMessage response)
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            string correlationId = default;

            if (response.Headers.TryGetValues(PexCorrelationIdHeaderName, out var correlationIdHeaders))
            {
                correlationId = correlationIdHeaders.FirstOrDefault();
            }

            return correlationId;
        }

        public static string GetPexCorrelationId(this HttpHeaders headers)
        {
            if (headers is null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            string correlationId = default;

            if (headers.TryGetValues(PexCorrelationIdHeaderName, out var correlationIdHeaders))
            {
                correlationId = correlationIdHeaders.FirstOrDefault();
            }

            return correlationId;
        }

        public static HttpRequestMessage SetPexCorrelationIdHeader(this HttpRequestMessage request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Headers.SetPexCorrelationIdHeader();

            return request;
        }

        public static HttpRequestMessage SetPexCorrelationIdHeader(this HttpRequestMessage request, string correlationId)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Headers.SetPexCorrelationIdHeader(correlationId);

            return request;
        }

        public static HttpRequestHeaders SetPexCorrelationIdHeader(this HttpRequestHeaders headers)
        {
            if (headers is null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            Activity.Current ??= new Activity("PEX CLIENT").Start();
            var correlationId = $"ext-{Activity.Current.TraceId}";

            headers.SetPexCorrelationIdHeader(correlationId);

            return headers;
        }

        public static HttpRequestHeaders SetPexCorrelationIdHeader(this HttpRequestHeaders headers, string correlationId)
        {
            if (headers is null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            headers.TryAddWithoutValidation(PexCorrelationIdHeaderName, correlationId);

            return headers;
        }

        public static HttpRequestMessage SetPexAuthorizationHeader(this HttpRequestMessage request, string externalToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Headers.SetPexAuthorizationHeader(externalToken);

            return request;
        }

        public static HttpRequestHeaders SetPexAuthorizationHeader(this HttpRequestHeaders headers, string externalToken)
        {
            if (headers is null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            return headers;
        }

        public static HttpRequestMessage SetPexAcceptJsonHeader(this HttpRequestMessage request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Headers.SetPexAcceptJsonHeader();

            return request;
        }

        public static HttpRequestHeaders SetPexAcceptJsonHeader(this HttpRequestHeaders headers)
        {
            if (headers is null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            headers.Accept.Clear();
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue(PexJsonMediaType));

            return headers;
        }

        public static HttpContent ToPexJsonContent<TData>(this TData bodyData)
        {
            return new StringContent(ToPexJson(bodyData), PexEncodingType, PexJsonMediaType);
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
