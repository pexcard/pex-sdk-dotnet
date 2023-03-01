namespace PexCard.Api.Client.Core.Models
{
    public class SpendingRulesetCategoriesModel
    {
        public int? CategoryId { get; set; }
        public bool? AssociationsOrganizationsAllowed { get; set; }
        public bool? AutomotiveDealersAllowed { get; set; }
        public bool? EducationalServicesAllowed { get; set; }
        public bool? EntertainmentAllowed { get; set; }
        public bool? FuelPumpsAllowed { get; set; }
        public bool? GasStationsConvenienceStoresAllowed { get; set; }
        public bool? GroceryStoresAllowed { get; set; }
        public bool? HealthcareChildcareServicesAllowed { get; set; }
        public bool? ProfessionalServicesAllowed { get; set; }
        public bool? RestaurantsAllowed { get; set; }
        public bool? RetailStoresAllowed { get; set; }
        public bool? TravelTransportationAllowed { get; set; }
        public bool? HardwareStoresAllowed { get; set; }

        public void SetCategoryByMccName(string name, bool? value)
        {
            switch (name)
            {
                case "Associations & Organizations":
                    AssociationsOrganizationsAllowed = value;
                    break;
                case "Automotive Dealers":
                    AutomotiveDealersAllowed = value;
                    break;
                case "Educational Services":
                    EducationalServicesAllowed = value;
                    break;
                case "Entertainment":
                    EntertainmentAllowed = value;
                    break;
                case "Fuel & Convenience Stores":
                    GasStationsConvenienceStoresAllowed = value;
                    break;
                case "Grocery Stores":
                    GroceryStoresAllowed = value;
                    break;
                case "Healthcare & Childcare Services":
                    HealthcareChildcareServicesAllowed = value;
                    break;
                case "Professional Services":
                    ProfessionalServicesAllowed = value;
                    break;
                case "Restaurants":
                    RestaurantsAllowed = value;
                    break;
                case "Retail Stores":
                    RetailStoresAllowed = value;
                    break;
                case "Travel & Transportation":
                    TravelTransportationAllowed = value;
                    break;
                case "Fuel Pumps Only":
                    FuelPumpsAllowed = value;
                    break;
                case "Hardware Stores":
                    HardwareStoresAllowed = value;
                    break;
            }
        }
    }

}
