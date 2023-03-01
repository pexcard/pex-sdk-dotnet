namespace PexCard.Api.Client.Core.Models
{
    public class CreateSpendingRulesetRequestModel
    {
        public string Name { get; set; }

        public decimal DailySpendLimit { get; set; }

        public SpendingRulesetCategoriesModel SpendingRulesetCategories { get; set; }

        public bool InternationalAllowed { get; set; }

        public bool CardNotPresentAllowed { get; set; }

        public CardPresence? CardPresence { get; set; }

        public bool UsePexAccountBalanceForAuths { get; set; }

        public bool UseCustomerAuthDecision { get; set; }

        public static CreateSpendingRulesetRequestModel WithDefaultRules(string name) => new CreateSpendingRulesetRequestModel()
        {
            Name = name,
            DailySpendLimit = 5_000,
            InternationalAllowed = true,
            CardNotPresentAllowed = true,
            CardPresence = Models.CardPresence.OnlyCardPresent,
            UsePexAccountBalanceForAuths = false,
            UseCustomerAuthDecision = false,
            SpendingRulesetCategories = new SpendingRulesetCategoriesModel
            {
                AssociationsOrganizationsAllowed = true,
                AutomotiveDealersAllowed = true,
                EducationalServicesAllowed = true,
                EntertainmentAllowed = true,
                FuelPumpsAllowed = true,
                GasStationsConvenienceStoresAllowed = true,
                GroceryStoresAllowed = true,
                HealthcareChildcareServicesAllowed = true,
                ProfessionalServicesAllowed = true,
                RestaurantsAllowed = true,
                RetailStoresAllowed = true,
                TravelTransportationAllowed = true,
                HardwareStoresAllowed = true,
            }
        };
    }
}
