using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PexCard.Api.Client.Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BillInboxSortBy
    {
        ReceivedDate = 10,
        Created = 20,
        Amount = 30,
        Status = 40,
        DueDate = 50,
        Vendor = 60,
        Source = 70
    }
}
