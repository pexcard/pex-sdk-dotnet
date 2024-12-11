using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public record PexRetryPolicyOptions
    {
        public static PexRetryPolicyOptions None => new PexRetryPolicyOptions
        {
            RetryLogLevel = LogLevel.None,
            TooManyRequests = BackoffRetryPolicy.None,
            Timeouts = BackoffRetryPolicy.None,
            ServerErrors = BackoffRetryPolicy.None,
        };

        public LogLevel RetryLogLevel { get; set; } = LogLevel.Warning;

        public BackoffRetryPolicy TooManyRequests { get; set; } = new BackoffRetryPolicy(TimeSpan.FromSeconds(5), 7);

        public BackoffRetryPolicy Timeouts { get; set; } = new BackoffRetryPolicy(TimeSpan.FromSeconds(1), 2);

        public BackoffRetryPolicy ServerErrors { get; set; } = new BackoffRetryPolicy(TimeSpan.FromMilliseconds(100), 1);
    }
}
