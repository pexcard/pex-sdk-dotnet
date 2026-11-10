namespace PexCard.Api.Client.Core.Enums
{
    /// <summary>
    /// Kind of a tag dependency, as returned in <c>TagDependencyModel.Type</c> and accepted
    /// by the <c>Types</c> filter of the business tag dependency list.
    /// </summary>
    public enum TagDependencyType
    {
        /// <summary>
        /// The options a dropdown tag offers depend on the value selected in another tag.
        /// </summary>
        Options = 1,

        /// <summary>
        /// Whether a tag is required depends on the value selected in another tag.
        /// </summary>
        Requirements = 2
    }
}
