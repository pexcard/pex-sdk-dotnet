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

        public int? MerchantId { get; set; }

        public bool SendNotification { get; set; }

        public bool VendorCardPaymentEnabled { get; set; }

        public bool AchPaymentEnabled { get; set; }
    }

    public class VendorAddressModel
    {
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }
    }

    public class VendorContactModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }
    }
}
