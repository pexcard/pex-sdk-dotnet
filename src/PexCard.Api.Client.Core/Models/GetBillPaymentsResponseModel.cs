using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class GetBillPaymentsResponseModel
    {
        public List<BillPaymentItemModel> Payments { get; set; }
    }
}
