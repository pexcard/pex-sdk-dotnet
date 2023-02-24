using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class VendorCardCreateOrderRequestModel
    {
        public List<VendorCardOrderItemRequest> VendorCards { get; set; }
    }
}
