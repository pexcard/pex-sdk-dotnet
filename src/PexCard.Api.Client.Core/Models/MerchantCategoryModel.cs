namespace PexCard.Api.Client.Core.Models
{
    public class MerchantCategoryModel
    {
        public int Id { get; set; }
        public int BusinessAcctId { get; set; }
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string[] MccCodes { get; set; }
        public string Description { get; set; }
        public bool IsPredefined { get; set; }
        public bool IsEditable { get; set; }
        public int CardholderCount { get; set; }
        public string AdminName { get; set; }
    }
}
