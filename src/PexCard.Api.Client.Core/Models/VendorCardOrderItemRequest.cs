namespace PexCard.Api.Client.Core.Models
{
    public class VendorCardOrderItemRequest
    {
        public string VendorName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? GroupId { get; set; }

        /// <summary>
        /// Id of the User Group to assign the new vendor card to. Honored only when the business has the
        /// cardholder-group feature enabled; otherwise it is nulled out server-side. When both
        /// <see cref="GroupId"/> and <see cref="UserGroupId"/> are supplied they must reference the same group.
        /// </summary>
        public long? UserGroupId { get; set; }
        public int? RulesetId { get; set; }
        public bool AutoActivation { get; set; } = true;
        public FundingType FundingType { get; set; }
        public decimal? FundCardAmount { get; set; }
        public string CardDataWebhookUrl { get; set; }
    }
}
