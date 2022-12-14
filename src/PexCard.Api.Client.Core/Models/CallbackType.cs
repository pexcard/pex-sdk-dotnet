using System.ComponentModel;

namespace PexCard.Api.Client.Core.Models
{
    public enum CallbackType
    {
        [Description("CARD")]
        Card = 1,
        [Description("CARDORDER")]
        Cardorder = 2,
        [Description("CARD-ORDER-VIRTUAL")]
        CardOrderVirtual = 3,
        [Description("CARD-TOKEN-CREATED")]
        CardTokenCreated = 4,
        [Description("AUTH-REALTIME")]
        AuthRealtime = 5,
        [Description("PIN-REALTIME")]
        PinRealtime = 6,
        [Description("DECLINE-REALTIME")]
        DeclineRealtime = 7,
        [Description("REVERSAL-REALTIME")]
        ReversalRealtime = 8,
        [Description("SETTLEMENT-POSTED-REALTIME")]
        SettlementPostedRealtime = 9
    }
}
