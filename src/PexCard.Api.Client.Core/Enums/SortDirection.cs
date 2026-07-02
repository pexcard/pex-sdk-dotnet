using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PexCard.Api.Client.Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SortDirection
    {
        Ascending = 1,
        Descending = 2
    }
}
