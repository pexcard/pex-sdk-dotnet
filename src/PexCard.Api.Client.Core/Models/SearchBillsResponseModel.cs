using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class SearchBillsResponseModel
    {
        public List<BillSearchItemModel> Items { get; set; }
        public SearchPageInfoModel PageInfo { get; set; }
    }
}
