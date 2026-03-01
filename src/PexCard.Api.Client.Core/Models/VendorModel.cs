using System;
using System.Collections.Generic;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class VendorModel
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string EmailForRemittance { get; set; }
        public string Website { get; set; }
        public string LogoUrl { get; set; }
        public VendorStatus VendorStatus { get; set; }
        public VendorStatusTrigger VendorStatusTrigger { get; set; }
        public bool? VendorCardPaymentEnabled { get; set; }
        public bool? AchPaymentEnabled { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public VendorAddressModel VendorAddress { get; set; }
        public VendorContactModel VendorContact { get; set; }
        public string CustomId { get; set; }
        public string Note { get; set; }
        public List<VendorDocumentModel> Documents { get; set; }
        public List<VendorCardModel> VendorCards { get; set; }
        public List<VendorBankAccountModel> BankAccounts { get; set; }
    }
}
