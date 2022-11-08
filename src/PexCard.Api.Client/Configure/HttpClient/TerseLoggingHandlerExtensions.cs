using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TerseLoggingHandlerExtensions
    {
        public static IHttpClientBuilder UsePexTerseLogging<TClient>(this IHttpClientBuilder builder, Func<IServiceProvider, (LogLevel Success, LogLevel Failure)> getLevels)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (getLevels is null)
            {
                throw new ArgumentNullException(nameof(getLevels));
            }

            return builder.AddHttpMessageHandler(sp =>
            {
                var levels = getLevels(sp);

                return new TerseLoggingHandler<TClient>(sp.GetRequiredService<ILogger<TClient>>(), levels.Success, levels.Failure);
            });
        }

        public static IHttpClientBuilder UsePexTerseLogging<TClient>(this IHttpClientBuilder builder, LogLevel successLogLevel = LogLevel.Information, LogLevel failureLogLevel = LogLevel.Warning)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.AddHttpMessageHandler(sp => new TerseLoggingHandler<TClient>(sp.GetRequiredService<ILogger<TClient>>(), successLogLevel, failureLogLevel));
        }
    }
}
