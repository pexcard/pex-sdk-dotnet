namespace PexCard.Api.Client.Core.Models
{
    public class PaginationInfoModel
    {
        public string SortBy { get; set; }

        public string OrderBy { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int ResultsCount { get; set; }

        public int? TotalCount { get; set; }
    }
}
