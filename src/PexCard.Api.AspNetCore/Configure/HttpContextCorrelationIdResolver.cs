using PexCard.Api.Client.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using System;

namespace PexCard.Api.AspNetCore
{
    public class HttpContextCorrelationIdResolver : ICorrelationIdResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CorrelationIdHeaderName = "X-Correlation-Id";
        private const string ContextItemKey = "PexCorrelationId";

        public HttpContextCorrelationIdResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public string CorrelationId => GetValue();

        public string GetValue()
        {
            var context = _httpContextAccessor.HttpContext;

            if (context?.Items.TryGetValue(ContextItemKey, out var cached) == true && cached is string cachedId)
            {
                return cachedId;
            }

            var correlationId = context?.Request?.Headers[CorrelationIdHeaderName].ToString();

            if (string.IsNullOrEmpty(correlationId))
            {
                correlationId = $"ext-{Guid.NewGuid()}";
            }

            if (context != null)
            {
                context.Items[ContextItemKey] = correlationId;
                context.Request.Headers[CorrelationIdHeaderName] = correlationId;
            }

            return correlationId;
        }
    }
}
