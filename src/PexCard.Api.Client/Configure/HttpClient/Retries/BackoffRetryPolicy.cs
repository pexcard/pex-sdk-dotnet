using Newtonsoft.Json;
using System;
using System.ComponentModel;

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

        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan Delay { get; set; } = TimeSpan.FromMilliseconds(100);

        public int Retries { get; set; } = 1;
    }
}
