using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PexCard.Api.Client;
using PexCard.Api.Client.Core;
using System;
using System.Net.Http.Headers;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigurePexAuthClientExtensions
    {
        public static IServiceCollection AddPexAuthClient(this IServiceCollection services) => AddPexAuthClient(services, new PexAuthClientOptions());

        public static IServiceCollection AddPexAuthClient(this IServiceCollection services, IConfiguration configSection)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configSection is null)
            {
                throw new ArgumentNullException(nameof(configSection));
            }

            services.Configure<PexAuthClientOptions>(configSection);

            return services.RegisterPexAuthClient();
        }

        public static IServiceCollection AddPexAuthClient(this IServiceCollection services, PexAuthClientOptions options)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return services.AddPexAuthClient((x) =>
            {
                x.BaseUri = options.BaseUri;
                x.Timeout = options.Timeout;
                x.Retries = options.Retries;
                x.LogLevelSuccess = options.LogLevelSuccess;
                x.LogLevelFailure = options.LogLevelFailure;
            });
        }

        public static IServiceCollection AddPexAuthClient(this IServiceCollection services, Action<PexAuthClientOptions> configure)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            services.Configure(configure);

            return services.RegisterPexAuthClient();
        }

        private static IServiceCollection RegisterPexAuthClient(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddHttpClient<IPexAuthClient, PexAuthClient>((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<PexAuthClientOptions>>().Value;

                httpClient.BaseAddress = options.BaseUri;
                httpClient.Timeout = options.Timeout;

                var sdkAssembly = typeof(PexApiClient).Assembly;
                var buildVersion = sdkAssembly.GetInformationalVersion() ?? sdkAssembly.GetVersion();
                var sdkUserAgent = new ProductInfoHeaderValue("pex-sdk", buildVersion);

                var appAssembly = Assembly.GetEntryAssembly();
                var appUserAgent = new ProductInfoHeaderValue(options.AppName ?? appAssembly.GetName().Name, options.AppVersion ?? appAssembly.GetVersion() ?? "0.0.0");

                httpClient.DefaultRequestHeaders.UserAgent.Add(sdkUserAgent);
                httpClient.DefaultRequestHeaders.UserAgent.Add(appUserAgent);
            })
            .UsePexTerseLogging<PexAuthClient>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<PexAuthClientOptions>>().Value;

                return (options.LogLevelSuccess, options.LogLevelFailure);
            })
            .UsePexRetryPolicies<PexAuthClient>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<PexAuthClientOptions>>().Value;

                return options.Retries;
            });

            return services;
        }
    }
}
