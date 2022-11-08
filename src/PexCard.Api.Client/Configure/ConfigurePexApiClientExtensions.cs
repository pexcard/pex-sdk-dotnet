using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PexCard.Api.Client;
using PexCard.Api.Client.Core;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigurePexApiClientExtensions
    {
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
                x.TimeOut = options.TimeOut;
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
                httpClient.Timeout = options.TimeOut;
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
    }
}
