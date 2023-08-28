using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;
using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

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

            IAsyncPolicy<HttpResponseMessage> wrappedPollyPolicies = null;

            foreach (var policy in options.Policies)
            {
                var pollyPolicy = Policy<HttpResponseMessage>
                    .HandleResult(resp => new Regex(policy.Regex).IsMatch(((int)resp.StatusCode).ToString()))
                    .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: policy.Delay, retryCount: policy.Retries), (result, retryDelay, retryNumber, ctx) => LogRetry(logger, policy.RetryLogLevel, result, retryDelay, retryNumber));

                if (wrappedPollyPolicies is null)
                {
                    wrappedPollyPolicies = pollyPolicy;
                }
                else
                {
                    wrappedPollyPolicies = wrappedPollyPolicies.WrapAsync(pollyPolicy);
                }
            }

            return wrappedPollyPolicies ?? Policy<HttpResponseMessage>
                    .HandleResult(resp => false)
                    .WaitAndRetryAsync(0, x => TimeSpan.Zero);
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
