using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace PexCard.Api.Client
{
    public class PexApiClientOptions
    {
#pragma warning disable S1075 // URIs should not be hardcoded
        public Uri BaseUri { get; set; } = new Uri("https://coreapi.pexcard.com");
#pragma warning restore S1075 // URIs should not be hardcoded

        [JsonConverter(typeof(JsonTimeSpanConverter))]
        public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(3);

        public LogLevel LogLevelSuccess { get; set; } = LogLevel.Debug;

        public LogLevel LogLevelFailure { get; set; } = LogLevel.Warning;

        public PexRetryPolicyOptions Retries { get; set; } = new PexRetryPolicyOptions();
    }
}
