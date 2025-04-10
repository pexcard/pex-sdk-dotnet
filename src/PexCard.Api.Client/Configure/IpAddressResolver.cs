using PexCard.Api.Client.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace PexCard.Api.Client.Configure
{
    public class IpAddressResolver : IIPAddressResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string PexForwardedForHeaderName = "X-Forwarded-For";

        public IpAddressResolver(IHttpContextAccessor httpContextAccessor)
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
