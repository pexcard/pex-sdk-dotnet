using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    public class BackoffRetryPolicy
    {
        public BackoffRetryPolicy()
        {
        }

        public BackoffRetryPolicy(string regex, TimeSpan delay, int retries, LogLevel retryLogLevel = LogLevel.Debug)
        {
            Regex = regex;
            Delay = delay;
            Retries = retries;
            RetryLogLevel = retryLogLevel;
        }

        public string Regex { get; set; }

        [JsonConverter(typeof(JsonTimeSpanConverter))]
        public TimeSpan Delay { get; set; } = TimeSpan.FromMilliseconds(100);

        public int Retries { get; set; } = 1;

        public LogLevel RetryLogLevel { get; set; } = LogLevel.Debug;
    }
}
