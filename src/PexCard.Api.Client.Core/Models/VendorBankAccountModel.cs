using System;

namespace PexCard.Api.Client.Core.Models
{
    public class VendorBankAccountModel
    {
        public int VendorId { get; set; }
        public long BankAccountId { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
    }
}
