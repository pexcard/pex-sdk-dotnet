using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class UpdateVendorStatusRequestModel
    {
        public VendorStatusUpdateTrigger NewStatus { get; set; }
    }
}
