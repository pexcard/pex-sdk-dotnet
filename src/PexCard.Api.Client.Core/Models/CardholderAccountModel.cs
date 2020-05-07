namespace PexCard.Api.Client.Core.Models
{
    public class CardholderAccountModel
    {
        /// <summary>
        /// Unique id for the cardholder account
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Cardholder first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Cardholder last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Ledger balance for the cardholder account, rounded to 2 decimal
        /// places
        /// </summary>
        public double LedgerBalance { get; set; }

        /// <summary>
        /// Available balance for the cardholder account, rounded to 2 decimal
        /// places
        /// </summary>
        public double AvailableBalance { get; set; }

        /// <summary>
        /// Cardholder account status (OPEN or CLOSED)
        /// </summary>
        public string AccountStatus { get; set; }

        /// <summary>
        /// Identifies virtual cardholder
        /// </summary>
        public bool? IsVirtual { get; set; }

        /// <summary>
        /// User defined Id which can be assigned to Card holder profile
        /// (alphanumeric up to 50 characters)
        /// </summary>
        public string CustomId { get; set; }
    }
}
