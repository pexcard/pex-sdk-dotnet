using System;
using System.Collections.Generic;
namespace PexCard.Api.Client.Core.Models
{
    public class CardholderProfileModel
    {
        public int AccountId { get; set; }
        public long? UserId { get; set; }
        public string AccountStatus { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        /// <summary>
        /// Legacy single cardholder group id.
        /// </summary>
        [Obsolete("Legacy single-group field. Use " + nameof(UserGroupId) + " / " + nameof(UserGroups) + " and the /UserGroup endpoints instead.")]
        public int CardholderGroupId { get; set; }

        /// <summary>
        /// Id of the User Group the cardholder belongs to, when applicable; otherwise null.
        /// </summary>
        public long? UserGroupId { get; set; }

        /// <summary>
        /// User Groups the cardholder belongs to.
        /// </summary>
        public List<UserGroupBrief> UserGroups { get; set; }

        public int SpendRulesetId { get; set; }
        public AddressContactModel ProfileAddress { get; set; }
        public string Phone { get; set; }
        public AddressContactModel ShippingAddress { get; set; }
        public string ShippingPhone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsVirtual { get; set; }
        public CardholderType CardholderType { get; set; }
        public string CustomId { get; set; }
    }
}
