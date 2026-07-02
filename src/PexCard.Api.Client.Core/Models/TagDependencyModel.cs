namespace PexCard.Api.Client.Core.Models
{
    public class TagDependencyModel
    {
        public string DependencyId { get; set; }
        public string DependentTagId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string DependsOnTagId { get; set; }
        public TagDependencyAuditModel Created { get; set; }
        public TagDependencyAuditModel Updated { get; set; }
    }
}
