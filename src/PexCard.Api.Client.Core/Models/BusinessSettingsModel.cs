using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class BusinessSettingsModel
    {
        public List<CardType> SupportedCardType { get; set; }
        public bool UseBusinessBalance { get; set; }
        public bool UseRemoteDecision { get; set; }
        public bool BlockCardDefunding { get; set; }
        public FundingSource FundingSource { get; set; }
    }
}