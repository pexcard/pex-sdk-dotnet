namespace PexCard.Api.Client.Core.Models
{
    /// <summary>
    /// Identifies what is creating or updating a tag dependency, as opposed to
    /// <see cref="TagDependencyAuditModel"/> which identifies who.
    /// Set this so the dependency is attributed to your integration rather than to the user
    /// whose token the request runs under.
    /// </summary>
    public class TagDependencySourceModel
    {
        /// <summary>
        /// The name of the integration making the change.
        /// PEX recognises a fixed set of names; an unrecognised one is recorded as unknown rather
        /// than rejected, so a typo fails silently.
        /// </summary>
        public string Name { get; set; }
    }
}
