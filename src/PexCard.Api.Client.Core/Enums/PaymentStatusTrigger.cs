namespace PexCard.Api.Client.Core.Enums
{
    public enum PaymentStatusTrigger
    {
        New = 10,

        Settling = 35,

        AwaitingOutboundCompletion = 40,

        Settled = 50,

        Returned = 51,

        Cancelled = 55
    }
}
