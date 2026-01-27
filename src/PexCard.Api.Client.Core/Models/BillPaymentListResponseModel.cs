using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class BillPaymentListResponseModel
    {
        public List<BillPaymentModel> Items { get; set; }

        public PageInfoModel PageInfo { get; set; }
    }
}
