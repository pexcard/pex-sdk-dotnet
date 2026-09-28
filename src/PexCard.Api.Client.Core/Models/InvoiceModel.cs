using System;
using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models
{
    public class InvoiceModel
    {
        public int InvoiceId { get; set; }
        public decimal InvoiceAmount { get; set; }
        public InvoiceStatus Status { get; set; }
        public DateTime DueDate { get; set; }
        public int BusinessAccountId { get; set; }
        public string Type { get; set; }
        public string InvoiceTypeName { get; set; }
        public string InvoiceDescription { get; set; }
        public decimal AmountTaken { get; set; }
        public decimal PendingAmount { get; set; }
        public decimal WaivedAmount { get; set; }
        public decimal RebateCreditAmount { get; set; }
        public decimal RebateCreditReversalAmount { get; set; }
        public decimal CarryOverCreditAmount { get; set; }
        public decimal WriteOffAmount { get; set; }
        public decimal WriteOffReversalAmount { get; set; }
        public string CCStatus { get; set; }
        public bool HasReversal { get; set; }
        public DateTime DateAssessed { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool IsPastReturnWindow { get; set; }
    }
}
