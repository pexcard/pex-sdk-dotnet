using System;

namespace PexCard.Api.Client.Core.Models
{
    public class VendorCardOrderItemResponse
    {
        public int RequestId { get; set; }
        public int? AcctId { get; set; }
        public string AccountNumber { get; set; }
        public string VendorName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public AddressModel HomeAddress { get; set; }
        public int? GroupId { get; set; }
        public int? SpendingRulesetsId { get; set; }
        public bool AutoActivation { get; set; }
        public decimal? FundCardAmount { get; set; }
        public string CardDataWebhookUrl { get; set; }
        public string Status { get; set; }
        public string[] Errors { get; set; }
        public string ErrorMessage { get; set; }
    }
}
