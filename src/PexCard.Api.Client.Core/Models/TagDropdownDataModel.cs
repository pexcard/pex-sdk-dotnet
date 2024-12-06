using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class TagDropdownDataModel : TagDetailsModel
    {
        public List<TagOptionModel> Options { get; set; }
    }
}
