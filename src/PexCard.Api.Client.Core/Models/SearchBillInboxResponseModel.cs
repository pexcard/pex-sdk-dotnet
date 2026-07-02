using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class SearchBillInboxResponseModel
    {
        public List<BillInboxModel> Items { get; set; }

        public PageInfoModel PageInfo { get; set; }
    }
}
