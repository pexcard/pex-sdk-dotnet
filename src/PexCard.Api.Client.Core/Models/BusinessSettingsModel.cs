using System.Collections.Generic;

namespace PexCard.Api.Client.Core.Models
{
    public class BusinessSettingsModel
    {
        public List<CardType> SupportedCardType { get; set; }
        public bool UseBusinessBalance { get; set; }
        public bool UseRemoteDecision { get; set; }
        public bool BlockCardDefunding { get; set; }
        public bool UseCardholderGroup { get; set; }
        public bool TagsEnabled { get; set; }
        public bool AllocationTagsEnabled { get; internal set; }
        public FundingSource FundingSource { get; set; }
        public int? CardLimit { get; set; }
        public decimal? DefaultDailyLimit { get; internal set; }
    }
}