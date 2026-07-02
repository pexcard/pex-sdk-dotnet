using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class TagOptionsDependencyModel : TagDependencyModel
    {
        public List<TagOptionsDependencyRuleModel> Rules { get; set; }
        public TagOptionsDependencyDefaultModel Default { get; set; }
    }
}
