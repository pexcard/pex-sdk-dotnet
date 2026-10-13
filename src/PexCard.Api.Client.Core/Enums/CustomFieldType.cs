using Newtonsoft.Json;
using PexCard.Api.Client.Core.Converters;

namespace PexCard.Api.Client.Core.Enums
{
    /// <summary>
    /// Tag type. Names the SDK does not know deserialize as <see cref="Unknown"/> instead of throwing;
    /// numbers the SDK does not know keep their value.
    /// </summary>
    [JsonConverter(typeof(TolerantCustomFieldTypeConverter))]
    public enum CustomFieldType
    {
        Unknown = -1,
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
