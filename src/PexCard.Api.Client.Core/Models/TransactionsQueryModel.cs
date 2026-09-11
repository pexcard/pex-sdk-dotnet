using System;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public abstract class TransactionsQueryModel
    {
        public const int DefaultPageIndex = 1;
        public const int DefaultPageSize = 100;
        public const int MaxPageSize = 1000;

        public DateTime? MinDate { get; set; }

        public DateTime? MaxDate { get; set; }

        public DateTime? OnDate { get; set; }

        public decimal? MinAmount { get; set; }

        public decimal? MaxAmount { get; set; }

        public decimal? EqualsAmount { get; set; }

        public TransactionQuerySortBy SortBy { get; set; } = TransactionQuerySortBy.TransactionTime;

        public TransactionQuerySortOrder OrderBy { get; set; } = TransactionQuerySortOrder.Desc;

        public int PageIndex { get; set; } = DefaultPageIndex;

        public int PageSize { get; set; } = DefaultPageSize;
    }
}
