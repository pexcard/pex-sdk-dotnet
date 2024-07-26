namespace PexCard.Api.Client.Core.Models
{
    public class LinkedBusinessModel
    {
        public int InstitutionId { get; set; }

        public int BusinessAcctId { get; set; }

        public string BusinessName { get; set; }

        public decimal MinBusinessBalance { get; set; }

        public decimal CurrentBalance { get; set; }
    }
}