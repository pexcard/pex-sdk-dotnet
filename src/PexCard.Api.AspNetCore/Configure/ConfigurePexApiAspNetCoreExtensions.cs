using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PexCard.Api.Client.Core.Interfaces;

namespace PexCard.Api.AspNetCore.Configure
{
    public static class ConfigurePexApiAspNetCoreExtensions
    {
        public static void AddPexHttpContextIpAddressResolver(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.RemoveAll<IIPAddressResolver>();
            services.AddScoped<IIPAddressResolver, IPAddressResolver>();
        }
    }
}
