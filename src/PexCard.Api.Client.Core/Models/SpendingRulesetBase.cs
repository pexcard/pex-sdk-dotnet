namespace PexCard.Api.Client.Core.Models
{
    public abstract class SpendingRulesetBase
    {
        public int BacctId { get; set; }

        public string Name { get; set; }

        public int CountCardsPresent { get; set; }

        public decimal DailySpendLimit { get; set; }

        public decimal WeeklySpendLimit { get; set; }

        public decimal MonthlySpendLimit { get; set; }

        public decimal YearlySpendLimit { get; set; }

        public decimal LifetimeSpendLimit { get; set; }

        public bool InternationalAllowed { get; set; }

        public bool CardNotPresentAllowed { get; set; }

        public CardPresence CardPresence { get; set; }

        public bool UsePexAccountBalanceForAuths { get; set; }

        public bool UseCustomerAuthDecision { get; set; }
    }

}
