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

        public BackoffRetryPolicy(TimeSpan delay, int retries)
        {
            Delay = delay;
            Retries = retries;
        }

        [JsonConverter(typeof(JsonTimeSpanConverter))]
        public TimeSpan Delay { get; set; } = TimeSpan.FromMilliseconds(100);

        public int Retries { get; set; } = 1;
    }
}
