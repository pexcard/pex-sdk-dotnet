using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class BillPaymentListResponseModel
    {
        public List<BillPaymentSearchItemModel> Items { get; set; }

        public PageInfoModel PageInfo { get; set; }
    }
}
