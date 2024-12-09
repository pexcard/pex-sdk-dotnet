using JsonSubTypes;
using Newtonsoft.Json;
using PexCard.Api.Client.Core.Enums;
using System;

namespace PexCard.Api.Client.Core.Models
{
    // dynamic deserialization into proper tag sub type classes based on 'CustomFieldType'
    // this will only occur when deserializing into TagDetailsModel or collection of TagDetailsModel.
    [JsonConverter(typeof(JsonSubtypes), nameof(Type))]
    [JsonSubtypes.KnownSubType(typeof(TagTextDetailsModel), CustomFieldType.Text)]
    [JsonSubtypes.KnownSubType(typeof(TagDropdownDetailsModel), CustomFieldType.Dropdown)]
    [JsonSubtypes.FallBackSubType(typeof(TagDetailsModel))]
    public class TagDetailsModel : TagModel
    {
        public string ConcurrencyKey { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public MetadataUserModel CreatedBy { get; set; }
        public DateTime? UpdatedDateUtc { get; set; }
        public MetadataUserModel UpdatedBy { get; set; }
        public DateTime? DeletedDateUtc { get; set; }
        public MetadataUserModel DeletedBy { get; set; }
    }
}
