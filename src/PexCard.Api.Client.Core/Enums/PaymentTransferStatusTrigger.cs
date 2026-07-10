namespace PexCard.Api.Client.Core.Enums
{
    public enum PaymentTransferStatusTrigger
    {
        New = 10,

        Edited = 15,

        RevisionsNeeded = 16,

        Submitted = 20,

        Approved = 30,

        Scheduled = 31,

        InboundAchCreationError = 33,

        Settling = 35,

        AwaitingPaymentDelay = 36,

        InboundAchCheckStatusError = 37,

        Settled = 50,

        Returned = 51,

        Rejected = 52,

        Cancelled = 55,

        InitiateTransferToPayablesError = 60,

        InitiatedTransferToPayables = 62
    }
}
