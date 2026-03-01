using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class GetVendorsResponseModel
    {
        public List<VendorPagedItemModel> Vendors { get; set; }
        public long TotalCount { get; set; }
    }
}
