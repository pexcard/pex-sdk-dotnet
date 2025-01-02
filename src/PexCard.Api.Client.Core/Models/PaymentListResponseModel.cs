using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class PaymentListResponseModel
    {
        public PageInfoModel PageInfo { get; set; }

        public List<PaymentModel> Items { get; set; }
    }
}
