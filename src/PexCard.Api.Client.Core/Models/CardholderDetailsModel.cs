using System;
using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class CardholderDetailsModel
    {
        public int AccountId { get; set; }
        public string AccountStatus { get; set; }
        public decimal LedgerBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public AddressContactModel ProfileAddress { get; set; }
        public string Phone { get; set; }
        public AddressContactModel ShippingAddress { get; set; }
        public string ShippingPhone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public bool IsVirtual { get; set; }
        public string CustomId { get; set; }
        public CardholderGroupModel Group { get; set; }
        public List<CardDetailModel> CardList { get; set; }
    }
}
