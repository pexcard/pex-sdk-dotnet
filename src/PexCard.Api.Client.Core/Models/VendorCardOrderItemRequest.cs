namespace PexCard.Api.Client.Core.Models
{
    public class VendorCardOrderItemRequest
    {
        public string VendorName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? GroupId { get; set; }
        public int? RulesetId { get; set; }
        public bool AutoActivation { get; set; } = true;
        public decimal? FundCardAmount { get; set; }
        public string CardDataWebhookUrl { get; set; }
    }
}
