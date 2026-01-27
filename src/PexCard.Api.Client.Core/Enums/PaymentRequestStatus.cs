namespace PexCard.Api.Client.Core.Enums
{
    public enum PaymentRequestStatus
    {
        Draft = 10,

        PendingApproval = 20,

        PendingPaymentTransfer = 30,

        PaymentTransferException = 35,

        PendingPayment = 40,

        Closed = 50,

        PaymentException = 55
    }
}
