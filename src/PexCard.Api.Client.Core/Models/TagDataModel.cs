using JsonSubTypes;
using Newtonsoft.Json;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    [JsonConverter(typeof(JsonSubtypes), nameof(Type))]
    [JsonSubtypes.KnownSubType(typeof(TagTextDataModel), CustomFieldType.Text)]
    [JsonSubtypes.KnownSubType(typeof(TagDropdownDataModel), CustomFieldType.Dropdown)]
    [JsonSubtypes.FallBackSubType(typeof(TagDetailsModel))]
    public abstract class TagDataModel
    {
        public string Name { get; set; }
        public CustomFieldType Type { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public bool IsRequired { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsDeleted { get; set; }
    }
}
