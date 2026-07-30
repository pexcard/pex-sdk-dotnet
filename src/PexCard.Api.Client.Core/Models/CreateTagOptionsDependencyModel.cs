using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class CreateTagOptionsDependencyModel
    {
        public string DependsOnTagId { get; set; }
        public List<CreateTagOptionsDependencyRuleModel> Rules { get; set; }
        public TagOptionsDependencyDefaultModel Default { get; set; }

        /// <summary>
        /// What is creating the dependency. Optional — leave it null and the dependency is
        /// attributed to the user whose token the request runs under.
        /// </summary>
        public TagDependencySourceModel Source { get; set; }
    }
}