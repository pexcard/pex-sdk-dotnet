using System.Collections.Generic;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class GetVendorsRequestModel
    {
        public int? CardholderAcctId { get; set; }
        public List<VendorStatus> VendorStatuses { get; set; }
        public List<VendorStatusTrigger> VendorStatusTriggers { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
