using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PexCard.Api.Client.Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionQuerySortOrder
    {
        Asc = 1,
        Desc = 2
    }
}
