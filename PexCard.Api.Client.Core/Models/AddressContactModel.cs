namespace PexCard.Api.Client.Core.Models
{
    public class AddressContactModel : AddressModel
    {
        public string ContactName { get; set; }

        public override string ToString()
        {
            return $"Address: {ContactName}/{AddressLine1}/{AddressLine2}/{City}/{State}/{PostalCode}/{Country}";
        }
    }
}
