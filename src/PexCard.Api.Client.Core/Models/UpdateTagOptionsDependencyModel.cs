using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class UpdateTagOptionsDependencyModel
    {
        public string DependsOnTagId { get; set; }
        public List<UpdateTagOptionsDependencyRuleModel> Rules { get; set; }
        public TagOptionsDependencyDefaultModel Default { get; set; }

        /// <summary>
        /// What is updating the dependency. Optional — leave it null and the update is attributed
        /// to the user whose token the request runs under.
        /// </summary>
        public TagDependencySourceModel Source { get; set; }
    }
}
