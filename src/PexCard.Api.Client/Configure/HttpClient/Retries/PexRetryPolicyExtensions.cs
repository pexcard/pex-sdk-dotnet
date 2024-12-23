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
        internal static class HttpRequestOptionsKeys
        {
            public static readonly string DontRetryRequest = nameof(DontRetryRequest);
        }

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

            return builder.AddPolicyHandler((sp, req) => GetRetryPolicy(sp, sp.GetRequiredService<ILoggerFactory>().CreateLogger<TClient>(), getOptions(sp)));
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

        internal static HttpRequestMessage DontRetryRequest(this HttpRequestMessage request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Properties[HttpRequestOptionsKeys.DontRetryRequest] = true;

            return request;
        }

        private static bool CanRetryRequest(this HttpRequestMessage request)
        {
            return request?.Properties.TryGetValue(HttpRequestOptionsKeys.DontRetryRequest, out var dontRetryRequest) != true || Equals(dontRetryRequest, false);
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

            var tooManyRequestsPolicy = Policy<HttpResponseMessage>
                .HandleResult(resp => resp.RequestMessage.CanRetryRequest() && (int)resp.StatusCode == 429)
                .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: options.TooManyRequests.Delay, retryCount: options.TooManyRequests.Retries), (result, retryDelay, retryNumber, ctx) => LogRetry(logger, options.RetryLogLevel, result, retryDelay, retryNumber));

            var timeoutPolicy = Policy<HttpResponseMessage>
                .HandleResult(resp => resp.RequestMessage.CanRetryRequest() && resp.RequestMessage?.Method == HttpMethod.Get && (resp.StatusCode == HttpStatusCode.RequestTimeout || resp.StatusCode == HttpStatusCode.GatewayTimeout))
                .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: options.Timeouts.Delay, retryCount: options.Timeouts.Retries), (result, retryDelay, retryNumber, ctx) => LogRetry(logger, options.RetryLogLevel, result, retryDelay, retryNumber));

            var serverErrorPolicy = Policy<HttpResponseMessage>
                .HandleResult(resp => resp.RequestMessage.CanRetryRequest() && resp.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: options.ServerErrors.Delay, retryCount: options.ServerErrors.Retries), (result, retryDelay, retryNumber, ctx) => LogRetry(logger, options.RetryLogLevel, result, retryDelay, retryNumber));

            return serverErrorPolicy.WrapAsync(timeoutPolicy).WrapAsync(tooManyRequestsPolicy);
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
