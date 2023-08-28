using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public class PexRetryPolicyOptions
    {
        public List<BackoffRetryPolicy> Policies { get; set; } = new List<BackoffRetryPolicy>
        {
            // 425: request too early, 429: too many requests
            new BackoffRetryPolicy("(?:425|429)", TimeSpan.FromSeconds(5), 7),

            // 408: request timeout, 502: bad gateway, 504: gateway timeout
            new BackoffRetryPolicy("(?:408|502|504)", TimeSpan.FromSeconds(3), 3),

            // 500: server error
            new BackoffRetryPolicy("(?:500)", TimeSpan.FromSeconds(1), 1),
        };
    }
}
