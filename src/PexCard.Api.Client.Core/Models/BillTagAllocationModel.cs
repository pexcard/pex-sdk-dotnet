using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class BillTagAllocationModel
    {
        public decimal Amount { get; set; }
        public List<BillTagValueModel> Tags { get; set; }
    }
}
