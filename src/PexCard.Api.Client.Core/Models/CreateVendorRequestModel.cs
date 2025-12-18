namespace PexCard.Api.Client.Core.Models
{
    public class CreateVendorRequestModel
    {
        public string VendorName { get; set; }
        public string EmailForRemittance { get; set; }
        public string WebSite { get; set; }
        public string CustomId { get; set; }
        public string TaxId { get; set; }
        public string Note { get; set; }
        public VendorAddressModel VendorAddress { get; set; }
        public VendorContactModel VendorContact { get; set; }
        public long? MerchantId { get; set; }
        public bool SendNotification { get; set; }
        public bool VendorCardPaymentEnabled { get; set; }
        public bool AchPaymentEnabled { get; set; }
    }
}
