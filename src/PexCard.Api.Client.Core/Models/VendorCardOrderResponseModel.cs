using System;
using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class VendorCardOrderResponseModel
    {
        public int CardOrderId { get; set; }
        public DateTime OrderDateTime { get; set; }
        public string UserName { get; set; }
        public List<VendorCardOrderItemResponse> Cards { get; set; }
    }
}
