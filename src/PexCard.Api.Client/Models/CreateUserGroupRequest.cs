namespace PexCard.Api.Client.Models
{
    /// <summary>
    /// Details required to create a User Group.
    /// </summary>
    public class CreateUserGroupRequest
    {
        /// <summary>
        /// User Group name (maximum 50 characters).
        /// </summary>
        public string Name { get; set; }
    }
}
