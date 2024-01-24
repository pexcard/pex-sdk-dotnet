using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PexCard.Api.Client;
using PexCard.Api.Client.Core;
using System;
using System.Net.Http.Headers;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigurePexApiClientExtensions
    {
        public static IServiceCollection AddPexApiClient(this IServiceCollection services) => AddPexApiClient(services, new PexApiClientOptions());

        public static IServiceCollection AddPexApiClient(this IServiceCollection services, IConfiguration configSection)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configSection is null)
            {
                throw new ArgumentNullException(nameof(configSection));
            }

            services.Configure<PexApiClientOptions>(configSection);

            return services.RegisterPexApiClient();
        }

        public static IServiceCollection AddPexApiClient(this IServiceCollection services, PexApiClientOptions options)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return services.AddPexApiClient((x) =>
            {
                x.BaseUri = options.BaseUri;
                x.Timeout = options.Timeout;
                x.Retries = options.Retries;
                x.LogLevelSuccess = options.LogLevelSuccess;
                x.LogLevelFailure = options.LogLevelFailure;
            });
        }

        public static IServiceCollection AddPexApiClient(this IServiceCollection services, Action<PexApiClientOptions> configure)
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

            return services.RegisterPexApiClient();
        }

        private static IServiceCollection RegisterPexApiClient(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddHttpClient<IPexApiClient, PexApiClient>((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<PexApiClientOptions>>().Value;

                httpClient.BaseAddress = options.BaseUri;
                httpClient.Timeout = options.Timeout;

                var sdkAssembly = typeof(PexApiClient).Assembly;
                var sdkUserAgentName = "pex-sdk";
                var sdkUserAgentVersion = FixUserAgentString(sdkAssembly.GetInformationalVersion() ?? sdkAssembly.GetVersion());
                var sdkUserAgent = new ProductInfoHeaderValue(sdkUserAgentName, sdkUserAgentVersion);

                var appAssembly = Assembly.GetEntryAssembly();
                var appUserAgentName = FixUserAgentString(options.AppName ?? appAssembly.GetName().Name);
                var appUserAgentVersion = FixUserAgentString(options.AppVersion ?? appAssembly.GetInformationalVersion() ?? appAssembly.GetVersion() ?? "0.0.0");
                var appUserAgent = new ProductInfoHeaderValue(appUserAgentName, appUserAgentVersion);

                httpClient.DefaultRequestHeaders.UserAgent.Add(sdkUserAgent);
                httpClient.DefaultRequestHeaders.UserAgent.Add(appUserAgent);
            })
            .UsePexTerseLogging<PexApiClient>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<PexApiClientOptions>>().Value;

                return (options.LogLevelSuccess, options.LogLevelFailure);
            })
            .UsePexRetryPolicies<PexApiClient>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<PexApiClientOptions>>().Value;

                return options.Retries;
            });

            return services;
        }

        private static string FixUserAgentString(string userAgentVersion)
        {
            // remove prohibited characters
            return userAgentVersion?.Replace(":", "");
        }
    }
}
