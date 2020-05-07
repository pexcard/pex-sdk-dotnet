namespace PexCard.Api.Client.Core.Models
{
    public class FundResponseModel
    {
        /// <summary>Unique id of the cardholder account</summary>
        public int AccountId { get; set; }

        /// <summary>Available balance for the card account, rounded to 2 decimal places</summary>
        public decimal AvailableBalance { get; set; }

        /// <summary>Ledger balance for the card account, rounded to 2 decimal places</summary>
        public decimal LedgerBalance { get; set; }
    }
}
