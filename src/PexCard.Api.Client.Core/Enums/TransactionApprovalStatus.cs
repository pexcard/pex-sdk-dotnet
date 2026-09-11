using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PexCard.Api.Client.Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionApprovalStatus
    {
        NotReviewed = 0,
        NoReceipt = 1,
        Approved = 2,
        Rejected = 3
    }
}
