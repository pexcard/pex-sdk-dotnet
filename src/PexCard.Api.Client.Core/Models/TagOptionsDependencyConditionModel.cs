using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class TagOptionsDependencyConditionModel
    {
        public string Type { get; set; }
        public List<string> TriggerOptions { get; set; }
    }
}
