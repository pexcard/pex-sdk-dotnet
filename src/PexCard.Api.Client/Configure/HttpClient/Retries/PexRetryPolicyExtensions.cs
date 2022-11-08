using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;
using System;
using System.Net;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PexRetryPolicyExtensions
    {
        public static IHttpClientBuilder UsePexRetryPolicies<TClient>(this IHttpClientBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.UsePexRetryPolicies<TClient>(new PexRetryPolicyOptions());
        }

        public static IHttpClientBuilder UsePexRetryPolicies<TClient>(this IHttpClientBuilder builder, Action<PexRetryPolicyOptions> configureOptions)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (configureOptions is null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            var options = new PexRetryPolicyOptions();

            configureOptions(options);

            return builder.AddPolicyHandler((sp, req) => GetRetryPolicy(sp, sp.GetRequiredService<ILoggerFactory>().CreateLogger<TClient>(), options));
        }

        public static IHttpClientBuilder UsePexRetryPolicies<TClient>(this IHttpClientBuilder builder, Func<IServiceProvider, PexRetryPolicyOptions> getOptions)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (getOptions is null)
            {
                throw new ArgumentNullException(nameof(getOptions));
            }

            return builder.UsePexRetryPolicies<TClient>(sp => getOptions(sp));
        }

        public static IHttpClientBuilder UsePexRetryPolicies<TClient>(this IHttpClientBuilder builder, PexRetryPolicyOptions options)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return builder.AddPolicyHandler((sp, req) => GetRetryPolicy(sp, sp.GetRequiredService<ILoggerFactory>().CreateLogger<TClient>(), options));
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(IServiceProvider serviceProvider, ILogger logger, PexRetryPolicyOptions options)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var tooManyRequestsPolicy = Policy<HttpResponseMessage>.Handle<HttpRequestException>()
                .OrResult(resp => (int)resp.StatusCode == 429)
                .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: options.TooManyRequests.Delay, retryCount: options.TooManyRequests.Retries), (result, retryDelay, retryNumber, ctx) => LogRetry(logger, options.RetryLogLevel, result, retryDelay, retryNumber));

            var timeoutPolicy = Policy<HttpResponseMessage>.Handle<HttpRequestException>()
                .OrResult(resp => resp.RequestMessage?.Method == HttpMethod.Get && (resp.StatusCode == HttpStatusCode.RequestTimeout || resp.StatusCode == HttpStatusCode.GatewayTimeout))
                .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: options.Timeouts.Delay, retryCount: options.Timeouts.Retries), (result, retryDelay, retryNumber, ctx) => LogRetry(logger, options.RetryLogLevel, result, retryDelay, retryNumber));

            var serverErrorPolicy = Policy<HttpResponseMessage>.Handle<HttpRequestException>()
                .OrResult(resp => resp.StatusCode >= HttpStatusCode.InternalServerError)
                .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: options.ServerErrors.Delay, retryCount: options.ServerErrors.Retries), (result, retryDelay, retryNumber, ctx) => LogRetry(logger, options.RetryLogLevel, result, retryDelay, retryNumber));

            return tooManyRequestsPolicy.WrapAsync(timeoutPolicy).WrapAsync(serverErrorPolicy);
        }

        private static void LogRetry(ILogger logger, LogLevel logLevel, DelegateResult<HttpResponseMessage> result, TimeSpan retryDelay, int retryNumber)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(logLevel, result?.Exception, "HTTP {RequestMethod} {RequestPath} responded {StatusCode}. Backing off {RetryNumber} for {RetryDelay} to retry.", result?.Result?.RequestMessage?.Method, result?.Result?.RequestMessage?.RequestUri, (int?)result?.Result?.StatusCode, retryNumber, retryDelay);
        }
    }
}
