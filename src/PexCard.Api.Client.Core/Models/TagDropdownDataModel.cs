using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class TagDropdownDataModel : TagDataModel
    {
        public List<TagOptionModel> Options { get; set; }
    }
}
