using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PexCard.Api.Client.Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BillInboxStatus
    {
        /// <summary>
        /// Forward-compat fallback for unknown values returned by the server.
        /// Not a valid value to send.
        /// </summary>
        Unknown = 0,
        Pending = 1,
        NeedsReview = 2,
        Approved = 3,
        Rejected = 4,
        Converted = 5
    }
}
