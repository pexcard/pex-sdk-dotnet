using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
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
