namespace PexCard.Api.Client.Core.Models
{
    public class AddVendorBankAccountRequestModel
    {
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankRoutingNumber { get; set; }
    }
}
