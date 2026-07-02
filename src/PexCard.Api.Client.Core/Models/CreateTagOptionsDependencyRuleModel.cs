using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class CreateTagOptionsDependencyRuleModel
    {
        public TagOptionsDependencyConditionModel Condition { get; set; }
        public List<string> AllowedOptions { get; set; }
    }
}
