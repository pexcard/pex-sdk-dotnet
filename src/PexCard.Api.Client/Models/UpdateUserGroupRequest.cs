namespace PexCard.Api.Client.Models
{
    /// <summary>
    /// Details required to update a User Group.
    /// </summary>
    public class UpdateUserGroupRequest
    {
        /// <summary>
        /// User Group name (maximum 50 characters).
        /// </summary>
        public string Name { get; set; }
    }
}
