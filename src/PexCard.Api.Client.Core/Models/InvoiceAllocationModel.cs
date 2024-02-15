namespace PexCard.Api.Client.Core.Models
{
    public class InvoiceAllocationModel
    {
        public long InvoiceId { get; set; }
        public string TagName { get; set; }
        public string TagValue { get; set; }
        public decimal TotalAmount { get; set; }
        public string TransactionTypeCategory { get; set; }
    }
}
