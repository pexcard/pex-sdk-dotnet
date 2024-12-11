using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    public record struct BackoffRetryPolicy
    {
        public static BackoffRetryPolicy None => new BackoffRetryPolicy(TimeSpan.MinValue, 0);

        public BackoffRetryPolicy()
        {
        }

        public BackoffRetryPolicy(BackoffRetryPolicy other)
        {
            Delay = other.Delay;
            Retries = other.Retries;
        }

        public BackoffRetryPolicy(TimeSpan delay, int retries)
        {
            Delay = delay;
            Retries = retries;
        }

        [JsonConverter(typeof(JsonTimeSpanConverter))]
        public TimeSpan Delay { get; set; } = TimeSpan.MinValue;

        public int Retries { get; set; } = 0;
    }
}
