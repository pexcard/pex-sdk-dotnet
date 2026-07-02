using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class TagOptionsDependencyRuleModel
    {
        public string RuleId { get; set; }
        public TagOptionsDependencyConditionModel Condition { get; set; }
        public List<string> AllowedOptions { get; set; }
    }
}
