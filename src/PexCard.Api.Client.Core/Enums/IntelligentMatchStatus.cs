using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PexCard.Api.Client.Core.Enums
{
    /// <summary>
    /// Intelligent (AI) match outcome produced by attachment analysis.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum IntelligentMatchStatus
    {
        /// <summary>No intelligent match outcome has been produced.</summary>
        Unknown,

        /// <summary>Analysis found no matching transaction.</summary>
        NoMatch,

        /// <summary>Analysis identified a matching transaction.</summary>
        Match,

        /// <summary>Analysis was skipped for this attachment.</summary>
        Skip
    }
}
