namespace PexCard.Api.Client.Core.Models
{
    public class ExternalBankAccount
    {
        public int ExternalBankAcctId { get; set; }

        public string RoutingNumber { get; set; }

        public string BankAccountNumber { get; set; }

        public string BankName { get; set; }

        public string BankAccountType { get; set; }

        public bool IsActive { get; set; }
    }
}