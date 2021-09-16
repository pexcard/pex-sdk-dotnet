using System;

namespace PexCard.Api.Client.Core.Models
{
    public class CardDetailModel
    {
        public int CardId { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Last4CardNumber { get; set; }
        public string CardStatus { get; set; }
    }
}
