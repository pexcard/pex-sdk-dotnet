namespace PexCard.Api.Client.Core.Models
{
    public class UpdateSpendingRulesetRequestModel
    {
        public int RulesetId { get; set; }

        public string Name { get; set; }

        public int CountCardsPresent { get; set; }

        public decimal DailySpendLimit { get; set; }

        public SpendingRulesetCategoriesModel SpendingRulesetCategories { get; set; }

        public bool MccRestrictions { get; set; }

        public bool InternationalAllowed { get; set; }

        public bool CardNotPresentAllowed { get; set; }

        public CardPresence? CardPresence { get; set; }

        public bool UsePexAccountBalanceForAuths { get; set; }

        public bool UseCustomerAuthDecision { get; set; }
    }
}
