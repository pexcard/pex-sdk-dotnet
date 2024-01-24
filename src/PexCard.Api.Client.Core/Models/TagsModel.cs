using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class TagsModel
    {
        public string ConcurrencyKey { get; set; }

        public List<TagValueModel> Values { get; set; }
    }
}
