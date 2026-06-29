namespace PexCard.Api.Client.Core.Models
{
    public class MetadataUserModel
    {
        /// <summary>Admin id, when the actor is an admin.</summary>
        public int? AdminId { get; set; }

        /// <summary>Cardholder/user account id, when the actor is a cardholder.</summary>
        public int? UserId { get; set; }

        /// <summary>Platform (security) user id.</summary>
        public long? PexUserId { get; set; }

        /// <summary>Login / username; null when the identity cannot be resolved.</summary>
        public string UserName { get; set; }

        /// <summary>First name; null when the identity cannot be resolved.</summary>
        public string FirstName { get; set; }

        /// <summary>Last name; null when the identity cannot be resolved.</summary>
        public string LastName { get; set; }
    }
}
