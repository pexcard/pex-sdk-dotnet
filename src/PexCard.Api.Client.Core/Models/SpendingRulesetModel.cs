namespace PexCard.Api.Client.Core.Models
{
    public class SpendingRulesetModel : SpendingRulesetBase
    {
        public int RulesetId { get; set; }

        public SpendingRulesetCategoriesModel SpendingRulesetCategories { get; set; }

        public bool MccRestrictions { get; set; }
    }
}
