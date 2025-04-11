using PexCard.Api.Client.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace PexCard.Api.AspNetCore
{
    public class IPAddressResolver : IIPAddressResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string PexForwardedForHeaderName = "X-Forwarded-For";

        public IPAddressResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
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