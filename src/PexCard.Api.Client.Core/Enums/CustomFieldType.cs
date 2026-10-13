using Newtonsoft.Json;
using PexCard.Api.Client.Core.Converters;

namespace PexCard.Api.Client.Core.Enums
{
    /// <summary>
    /// Tag type. Values the SDK does not know (for example a type added to the API after this
    /// release) deserialize as <c>(CustomFieldType)(-1)</c> instead of throwing.
    /// </summary>
    [JsonConverter(typeof(TolerantCustomFieldTypeConverter))]
    public enum CustomFieldType
    {
        Text = 0,
        YesNo = 1,
        Dropdown = 2,
        Decimal = 3,
        PercentageTax = 4,
        AbsoluteTax = 5,
        MerchantAddress = 6,
        Odometer = 7,
        Allocation = 10
    }
}
