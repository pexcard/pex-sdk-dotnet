namespace PexCard.Api.Client.Core.Models
{
    /// <summary>
    /// Extracted/matched fields from attachment analysis. Each criterion is populated only when detected.
    /// </summary>
    public class AttachmentMatchCriteriaModel
    {
        public AttachmentMatchCriterionModel Merchant { get; set; }
        public AttachmentMatchCriterionModel Total { get; set; }
        public AttachmentMatchCriterionModel Date { get; set; }
        public AttachmentMatchCriterionModel Time { get; set; }
        public AttachmentMatchCriterionModel OrderDate { get; set; }
        public AttachmentMatchCriterionModel OrderTime { get; set; }
        public AttachmentMatchCriterionModel Last4Digits { get; set; }
        public AttachmentMatchCriterionModel Subtotal { get; set; }
        public AttachmentMatchCriterionModel AmountPaid { get; set; }
        public AttachmentMatchCriterionModel Gratuity { get; set; }
        public AttachmentMatchCriterionModel InvoiceReceiptId { get; set; }
        public AttachmentMatchCriterionModel Pump { get; set; }
        public AttachmentMatchCriterionModel Gallons { get; set; }
        public AttachmentMatchCriterionModel PricePerGallon { get; set; }
        public AttachmentMatchCriterionModel FuelProduct { get; set; }
        public AttachmentMatchCriterionModel Currency { get; set; }
        public AttachmentMatchCriterionModel Mcc { get; set; }
        public AttachmentMatchCriterionModel IsFuel { get; set; }
        public AttachmentMatchCriterionModel IsForeignCurrency { get; set; }
        public AttachmentMatchCriterionModel IsReceipt { get; set; }
        public AttachmentMatchCriterionModel IsInvoice { get; set; }
        public AttachmentMatchCriterionModel IsHotel { get; set; }
        public AttachmentMatchCriterionModel IsAirTrainTravel { get; set; }
        public AttachmentMatchCriterionModel IsAirfare { get; set; }
        public AttachmentMatchCriterionModel IsVehicleRental { get; set; }
        public AttachmentMatchCriterionModel IsRideShare { get; set; }
        public AttachmentMatchCriterionModel DueDate { get; set; }
        public AttachmentMatchCriterionModel PaymentTerms { get; set; }
        public AttachmentMatchCriterionModel RemitToName { get; set; }
        public AttachmentMatchCriterionModel RemitToAddress { get; set; }
        public AttachmentMatchCriterionModel RemitToEmail { get; set; }
        public AttachmentMatchCriterionModel BankDetails { get; set; }
        public AttachmentMatchCriterionModel RemitToWebsite { get; set; }
        public AttachmentMatchCriterionModel RemitToTaxId { get; set; }
        public AttachmentMatchCriterionModel RemitToAddress1 { get; set; }
        public AttachmentMatchCriterionModel RemitToAddress2 { get; set; }
        public AttachmentMatchCriterionModel RemitToCity { get; set; }
        public AttachmentMatchCriterionModel RemitToState { get; set; }
        public AttachmentMatchCriterionModel RemitToPostalCode { get; set; }
        public AttachmentMatchCriterionModel RemitToContactFirstName { get; set; }
        public AttachmentMatchCriterionModel RemitToContactLastName { get; set; }
        public AttachmentMatchCriterionModel RemitToContactPhoneNumber { get; set; }
        public AttachmentMatchCriterionModel RemitToContactEmail { get; set; }
        public AttachmentMatchCriterionModel RemitToBankName { get; set; }
        public AttachmentMatchCriterionModel RemitToBankAccountNumber { get; set; }
        public AttachmentMatchCriterionModel RemitToBankRoutingNumber { get; set; }
    }
}
