using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class VendorListResponseModel
    {
        public List<VendorModel> Vendors { get; set; }

        public int TotalCount { get; set; }
    }
}
