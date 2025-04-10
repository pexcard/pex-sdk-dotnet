using PexCard.Api.Client.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace PexCard.Api.Client.Configure
{
    public class IpAddressResolver : IIPAddressResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IpAddressResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetValue()
        {
            var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            return ipAddress;
        }
    }
}
