using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class VendorModel
    {
        public int VendorId { get; set; }

        public string VendorName { get; set; }

        public string IconUrl { get; set; }

        public VendorStatus VendorStatus { get; set; }

        public VendorStatusTrigger VendorStatusTrigger { get; set; }

        public int CreatedByUserId { get; set; }

        public string Owner { get; set; }

        public string CustomId { get; set; }
    }
}
