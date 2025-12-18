namespace PexCard.Api.Client.Core.Models
{
    public class VendorCardModel
    {
        public int CardholderAcctId { get; set; }
        public string CardNumber4Digits { get; set; }
        public string CardStatus { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsDefault { get; set; }
    }
}
