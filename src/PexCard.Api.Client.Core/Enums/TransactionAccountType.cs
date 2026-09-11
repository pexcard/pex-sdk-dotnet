using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PexCard.Api.Client.Core.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionAccountType
    {
        Business = 1,
        Cardholder = 2,
        Bank = 3
    }
}
