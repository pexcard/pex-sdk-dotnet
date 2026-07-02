using System;
using System.Collections.Generic;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class SearchBillInboxRequestModel
    {
        /// <summary>
        /// Filter by status.
        /// </summary>
        public List<BillInboxStatus> Statuses { get; set; }

        /// <summary>
        /// Filter by source app.
        /// </summary>
        public BillInboxSource? Source { get; set; }

        public DateTimeOffset? ReceivedDateFrom { get; set; }

        public DateTimeOffset? ReceivedDateTo { get; set; }

        public DateTimeOffset? DueDateFrom { get; set; }

        public DateTimeOffset? DueDateTo { get; set; }

        public DateTimeOffset? BillDateFrom { get; set; }

        public DateTimeOffset? BillDateTo { get; set; }

        public SortDirection? SortDirection { get; set; }

        public BillInboxSortBy? SortColumn { get; set; }
    }
}
