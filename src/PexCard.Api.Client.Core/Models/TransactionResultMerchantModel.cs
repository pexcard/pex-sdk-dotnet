using System;

namespace PexCard.Api.Client.Core.Models
{
    public class TransactionResultMerchantModel
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string NameNormalized { get; set; }

        public string MccCode { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Zip { get; set; }

        public Uri WebsiteUrl { get; set; }

        public Uri LogoUrl { get; set; }

        public Uri IconUrl { get; set; }
    }
}
