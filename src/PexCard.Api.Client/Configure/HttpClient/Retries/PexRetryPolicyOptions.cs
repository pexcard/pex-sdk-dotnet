using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public record PexRetryPolicyOptions
    {
        public static PexRetryPolicyOptions None => new PexRetryPolicyOptions
        {
            RetryLogLevel = LogLevel.Warning,
            TooManyRequests = BackoffRetryPolicy.None,
            Timeouts = BackoffRetryPolicy.None,
            ServerErrors = BackoffRetryPolicy.None,
        };

        public static PexRetryPolicyOptions Default => new PexRetryPolicyOptions
        {
            RetryLogLevel = LogLevel.Warning,
            TooManyRequests = new BackoffRetryPolicy(TimeSpan.FromSeconds(5), 7),
            Timeouts = new BackoffRetryPolicy(TimeSpan.FromSeconds(1), 2),
            ServerErrors = new BackoffRetryPolicy(TimeSpan.FromMilliseconds(100), 1),
        };

        public LogLevel RetryLogLevel { get; set; } = LogLevel.Warning;

        public BackoffRetryPolicy TooManyRequests { get; set; } = new BackoffRetryPolicy();

        public BackoffRetryPolicy Timeouts { get; set; } = new BackoffRetryPolicy();

        public BackoffRetryPolicy ServerErrors { get; set; } = new BackoffRetryPolicy();
    }
}
