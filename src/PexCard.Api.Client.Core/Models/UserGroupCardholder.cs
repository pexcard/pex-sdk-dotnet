namespace PexCard.Api.Client.Core.Models
{
    /// <summary>
    /// A cardholder member of a User Group, identified by cardholder account id.
    /// </summary>
    public class UserGroupCardholder
    {
        /// <summary>
        /// Account id of the member.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Member first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Member last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Member user name (login/email).
        /// </summary>
        public string UserName { get; set; }
    }
}
