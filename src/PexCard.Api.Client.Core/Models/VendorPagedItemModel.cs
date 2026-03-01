using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class VendorPagedItemModel
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string IconUrl { get; set; }
        public VendorStatus VendorStatus { get; set; }
        public VendorStatusTrigger VendorStatusTrigger { get; set; }
        public long CreatedByUserId { get; set; }
        public string Owner { get; set; }
    }
}
