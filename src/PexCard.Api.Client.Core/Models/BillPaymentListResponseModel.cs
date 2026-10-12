using System.Collections.Generic;
using Newtonsoft.Json;
using PexCard.Api.Client.Core.Converters;

namespace PexCard.Api.Client.Core.Models
{
    public class BillPaymentListResponseModel
    {
        public List<BillPaymentModel> Items { get; set; }

        [JsonConverter(typeof(SearchPageInfoConverter))]
        public PageInfoModel PageInfo { get; set; }
    }
}
