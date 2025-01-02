namespace PexCard.Api.Client.Core.Enums
{
    public enum PaymentRequestStatusTrigger
    {
        New = 10,

        Edited = 15,

        RevisionsNeeded = 16,

        UpdatedAmount = 17,

        Submitted = 20,

        Approved = 30,

        IncludedInPaymentTransfer = 35,

        RemovedFromPaymentTransfer = 36,

        PaymentTransferFailed = 37,

        PaymentTransferCompleted = 40,

        PaymentInitiated = 42,

        Paid = 50,

        Rejected = 52,

        PaymentFailed = 53,

        Cancelled = 55
    }
}
