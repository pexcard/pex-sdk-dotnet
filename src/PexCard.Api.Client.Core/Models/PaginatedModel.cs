using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class PaginatedModel<T>
    {
        public IList<T> Results { get; set; } = new List<T>();

        public PaginationInfoModel Pagination { get; set; }
    }
}
