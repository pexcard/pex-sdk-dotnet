using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class CreditPaymentListResponseModel
    {
        public List<CreditPaymentModel> Items { get; set; }

        public PageInfoModel PageInfo { get; set; }
    }
}
