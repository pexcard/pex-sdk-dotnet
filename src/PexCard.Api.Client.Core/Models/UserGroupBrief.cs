namespace PexCard.Api.Client.Core.Models
{
    /// <summary>
    /// Summary of a User Group. User Groups are the forward-looking, many-to-many
    /// replacement for the legacy 1:1 cardholder Group model.
    /// </summary>
    public class UserGroupBrief
    {
        /// <summary>
        /// Unique id of the User Group.
        /// </summary>
        public long UserGroupId { get; set; }

        /// <summary>
        /// User Group name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of the group. Kept as a string to mirror the External API contract (which serializes
        /// the group-type enum by name) and to stay forward-compatible if new group types are added.
        /// </summary>
        public string GroupType { get; set; }

        /// <summary>
        /// Legacy group id this User Group corresponds to, when applicable; otherwise null.
        /// </summary>
        public int? LegacyGroupId { get; set; }
    }
}
