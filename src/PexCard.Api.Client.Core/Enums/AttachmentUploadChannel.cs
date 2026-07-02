using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PexCard.Api.Client.Core.Enums
{
    /// <summary>
    /// Channel a business source attachment (an emailed / SMS'd receipt or invoice) originated from.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AttachmentUploadChannel
    {
        /// <summary>
        /// The attachment was received by email; <c>Source</c> is the sender's email address.
        /// </summary>
        Email,

        /// <summary>
        /// The attachment was received by SMS; <c>Source</c> is the sender's phone number.
        /// </summary>
        Sms
    }
}
