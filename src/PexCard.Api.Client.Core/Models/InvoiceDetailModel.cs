using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class InvoiceDetailModel : InvoiceModel
    {
        public InvoicePartyModel Payer { get; set; }
        public InvoicePartyModel Payee { get; set; }
        public List<InvoiceLineModel> Lines { get; set; } = new List<InvoiceLineModel>();
    }
}
