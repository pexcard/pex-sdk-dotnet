using System;
using System.Collections.Generic;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class SearchBillsRequestModel
    {
        public DateTimeOffset? CreatedDateFrom { get; set; }
        public DateTimeOffset? CreatedDateTo { get; set; }
        public DateTimeOffset? DueDateFrom { get; set; }
        public DateTimeOffset? DueDateTo { get; set; }
        public long? CreatedByUserId { get; set; }
        public int? VendorId { get; set; }
        public decimal? AmountFrom { get; set; }
        public decimal? AmountTo { get; set; }
        public List<string> PaymentRequestStatuses { get; set; }
        public List<string> PaymentRequestStatusTriggers { get; set; }
        public bool IsOwner { get; set; }
        public bool IsPendingReview { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public BillSortDirection? SortDirection { get; set; }
        public BillSortField? SortByField { get; set; }
    }
}
