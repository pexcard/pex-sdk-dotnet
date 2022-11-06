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
    }
}
