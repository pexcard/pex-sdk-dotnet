using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class BillTagsModel
    {
        public bool IsSplit { get; set; }
        public List<BillTagAllocationResponseModel> Allocations { get; set; }
    }

    public class BillTagAllocationResponseModel
    {
        public decimal Amount { get; set; }
        public List<BillTagValueResponseModel> Tags { get; set; }
    }

    public class BillTagValueResponseModel
    {
        public string TagId { get; set; }
        public string TagName { get; set; }
        public object TagOptionValue { get; set; }
        public string TagOptionName { get; set; }
    }
}
