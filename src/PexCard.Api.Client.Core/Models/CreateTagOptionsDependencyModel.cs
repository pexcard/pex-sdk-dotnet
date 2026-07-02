using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class CreateTagOptionsDependencyModel
    {
        public string DependsOnTagId { get; set; }
        public List<CreateTagOptionsDependencyRuleModel> Rules { get; set; }
        public TagOptionsDependencyDefaultModel Default { get; set; }
    }
}