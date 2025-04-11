using PexCard.Api.Client.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using System;

namespace PexCard.Api.AspNetCore
{
    public class HttpContextIpAddressResolver : IIPAddressResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string PexForwardedForHeaderName = "X-Forwarded-For";

        public HttpContextIpAddressResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public string GetValue()
        {
            var ipAddress =
                _httpContextAccessor.HttpContext?.Request?.Headers[PexForwardedForHeaderName] ??
                _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            return ipAddress;
        }
    }
}