namespace PexCard.Api.Client.Core.Enums
{
    public enum PaymentStatusTrigger
    {
        New = 10,
        OutboundAchCreationError = 30,
        Settling = 35,
        OutBoundAchCheckStatusError = 36,
        AwaitingOutboundCompletion = 40,
        Settled = 50,
        Returned = 51,
        Cancelled = 55,
        InProgress = 56,

        //VirtualCard
        VirtualCardCreated = 60,
        VirtualCardEnsureSpendingRuleset = 61,
        VirtualCardDeliveryCreated = 65,
        VirtualCardDetailsRequestCreated = 66,
        VirtualCardCreationError = 70,
        VirtualCardEnsureSpendingRulesetError = 71,
        VirtualCardDeliveryCreationError = 75,
        VirtualCardDetailsRequestCreationError = 76
    }
}
