using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class UpdateBillTagsRequestModel
    {
        public List<BillTagAllocationModel> Tags { get; set; }
    }
}
