namespace PexCard.Api.Client.Core.Models
{
    public class UpdateVendorRequestModel
    {
        public string VendorName { get; set; }
        public string EmailForRemittance { get; set; }
        public string WebSite { get; set; }
        public string CustomId { get; set; }
        public string TaxId { get; set; }
        public string Note { get; set; }
        public VendorAddressModel VendorAddress { get; set; }
        public VendorContactModel VendorContact { get; set; }
        public bool? VendorCardPaymentEnabled { get; set; }
    }
}
