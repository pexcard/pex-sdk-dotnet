using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PexCard.Api.Client.Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionQuerySortBy
    {
        TransactionTime = 0,
        Amount = 1,
        Description = 2
    }
}
