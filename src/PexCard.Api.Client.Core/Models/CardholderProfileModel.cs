using System;
namespace PexCard.Api.Client.Core.Models
{
    public class CardholderProfileModel
    {
        public int AccountId { get; set; }
        public string AccountStatus { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CardholderGroupId { get; set; }
        public int SpendRulesetId { get; set; }
        public AddressContactModel ProfileAddress { get; set; }
        public string Phone { get; set; }
        public AddressContactModel ShippingAddress { get; set; }
        public string ShippingPhone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public bool IsVirtual { get; set; }
        public CardholderType CardholderType { get; set; }
        public string CustomId { get; set; }
    }
}
