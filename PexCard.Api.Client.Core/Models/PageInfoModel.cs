using System;

namespace PexCard.Api.Client.Core.Models
{
    public class PageInfoModel
    {
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int NumberOfPages { get; set; }
        public DateTime LastTimeRetrieved { get; set; }
    }
}