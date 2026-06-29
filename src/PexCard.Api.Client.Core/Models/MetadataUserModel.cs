namespace PexCard.Api.Client.Core.Models
{
    public class MetadataUserModel
    {
        public int? AdminId { get; set; }
        public int? UserId { get; set; }
        public long? PexUserId { get; set; }

        /// <summary>Login / username of the user, resolved by the External API from the admin/cardholder id; null when not resolvable.</summary>
        public string UserName { get; set; }

        /// <summary>First name of the user, resolved by the External API; null when not resolvable.</summary>
        public string FirstName { get; set; }

        /// <summary>Last name of the user, resolved by the External API; null when not resolvable.</summary>
        public string LastName { get; set; }
    }
}
