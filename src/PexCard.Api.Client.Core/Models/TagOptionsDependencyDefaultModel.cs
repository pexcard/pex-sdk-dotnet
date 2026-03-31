using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class TagOptionsDependencyDefaultModel
    {
        public bool AllowedAll { get; set; }
        public List<string> AllowedOptions { get; set; }
    }
}
