﻿using System;
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

        public static HttpRequestMessage SetPexJsonContent<TData>(this HttpRequestMessage request, TData contentData)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Content = contentData.ToPexJsonContent();

            return request;
        }

        public static void SetPexCorrelationIdHeader(this HttpRequestMessage request, string correlationId)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Headers.TryAddWithoutValidation(PexCorrelationIdHeaderName, correlationId);
        }

        public static void SetXForwardForHeader(this HttpRequestMessage request, string forwardFor)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (!string.IsNullOrWhiteSpace(forwardFor))
                request.Headers.TryAddWithoutValidation(PexForwardedForHeaderName, forwardFor);
        }

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

        public static HttpRequestMessage SetPexAuthorizationBearerHeader(this HttpRequestMessage request, string bearerToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Headers.SetPexAuthorizationBearerHeader(bearerToken);

            return request;
        }

        public static HttpRequestMessage SetPexAuthorizationTokenHeader(this HttpRequestMessage request, string externalToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Headers.SetPexAuthorizationTokenHeader(externalToken);

            return request;
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

        public static bool IsPexJsonContent(this HttpResponseMessage response)
        {
            return response.Headers.IsPexJsonContent();
        }

        public static bool IsPexJsonContent(this HttpHeaders headers)
        {
            return headers.TryGetValues("Content-Type", out var contentTypes) && contentTypes.Contains(PexJsonMediaType);
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

        #region Private Methods

        private static HttpRequestHeaders SetPexAuthorizationBearerHeader(this HttpRequestHeaders headers, string bearerToken)
        {
            if (headers is null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            headers.Authorization = new AuthenticationHeaderValue(TokenType.Bearer, bearerToken);

            return headers;
        }

        private static HttpRequestHeaders SetPexAuthorizationTokenHeader(this HttpRequestHeaders headers, string externalToken)
        {
            if (headers is null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            return headers;
        }

        private static HttpRequestHeaders SetPexAcceptJsonHeader(this HttpRequestHeaders headers)
        {
            if (headers is null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            headers.Accept.Clear();
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue(PexJsonMediaType));

            return headers;
        }

        #endregion
    }
}
