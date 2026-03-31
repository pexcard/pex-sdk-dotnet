using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class UpdateTagOptionsDependencyModel
    {
        public string DependsOnTagId { get; set; }
        public List<UpdateTagOptionsDependencyRuleModel> Rules { get; set; }
        public TagOptionsDependencyDefaultModel Default { get; set; }
    }
}
