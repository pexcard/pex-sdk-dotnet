using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PexCard.Api.Client.Core.Enums;
using PexCard.Api.Client.Core.Models;

namespace PexCard.Api.Client.Core
{
    public interface IPexApiClient
    {
        Uri BaseUri { get; }

        Task<bool> Ping(CancellationToken cancelToken = default);

        Task<string> ExchangeJwtForApiToken(string jwt, ExchangeTokenRequestModel exchangeTokenRequest, CancellationToken cancelToken = default);

        Task<TokenDataModel> GetToken(string externalToken, CancellationToken cancelToken = default);

        Task<TokenResponseModel> GetTokens(string externalToken, CancellationToken cancelToken = default);

        Task<RenewTokenResponseModel> RenewExternalToken(string externalToken, CancellationToken cancelToken = default);

        Task DeleteExternalToken(string externalToken, CancellationToken cancelToken = default);

        Task<PartnerModel> GetPartner(string externalToken, CancellationToken cancelToken = default);

        Task<BusinessDetailsModel> GetBusinessDetails(string externalToken, CancellationToken cancelToken = default);

        Task<BusinessProfileModel> GetBusinessProfile(string externalToken, CancellationToken cancelToken = default);

        Task<BusinessSettingsModel> GetBusinessSettings(string externalToken, CancellationToken cancelToken = default);

        Task<List<LinkedBusinessModel>> GetLinkedBusinesses(string externalToken, CancellationToken cancelToken = default);

        Task<BusinessAdminReponseModel> GetMyAdminProfile(string externalToken, CancellationToken cancelToken = default);

        Task<BusinessAdminsReponseModel> GetBusinessAdmins(string externalToken, CancellationToken cancelToken = default);

        Task<BusinessAdminReponseModel> GetBusinessAdmin(string externalToken, long adminId, CancellationToken cancelToken = default);

        Task<BusinessAdminReponseModel> CreateBusinessAdmin(string externalToken, CreateBusinessAdminModel newAdmin, CancellationToken cancelToken = default);

        Task<BusinessAdminReponseModel> UpdateBusinessAdmin(string externalToken, long adminId, UpdateBusinessAdminModel updateAdmin, CancellationToken cancelToken = default);

        Task DeleteBusinessAdmin(string externalToken, long adminId, CancellationToken cancelToken = default);

        Task<decimal> GetPexAccountBalance(string externalToken, CancellationToken cancelToken = default);

        Task<BusinessAccountTransactions> GetBusinessAccountTransactions(string externalToken, DateTime startDate, DateTime endDate, bool includePendings = false, bool includeDeclines = false, CancellationToken cancelToken = default);

        Task<int> GetAllCardholderTransactionsCount(string externalToken, DateTime startDate, DateTime endDate, bool includePendings = false, bool includeDeclines = false, CancellationToken cancelToken = default);

        Task<CardholderTransactions> GetAllCardholderTransactions(string externalToken, DateTime startDate, DateTime endDate, bool includePendings = false, bool includeDeclines = false, CancellationToken cancelToken = default);

        Task<CardholderTransactions> GetCardholderTransactions(string externalToken, int cardholderAccountId, DateTime startDate, DateTime endDate, bool includePending = false, bool includeDeclines = false, CancellationToken cancelToken = default);

        Task AddTransactionNote(string externalToken, TransactionModel transaction, string noteText, bool visibleToCardholder = false, bool systemGenerated = true, CancellationToken cancelToken = default);

        Task AddTransactionRelationshipNote(string externalToken, long transactionRelationshipId, string noteText, CancellationToken cancelToken = default);

        Task UpdateTransactionNote(string externalToken, long noteId, string noteText, bool isPending, CancellationToken cancelToken = default);

        Task DeleteTransactionNote(string externalToken, long noteId, bool isPending, CancellationToken cancelToken = default);

        Task<List<AttachmentLinkModel>> GetTransactionAttachments(string externalToken, long transactionId, CancellationToken cancelToken = default);

        Task<AttachmentModel> GetTransactionAttachment(string externalToken, long transactionId, string attachmentId, AttachmentLinkType attachmentLinkType = AttachmentLinkType.LinkUrl, CancellationToken cancelToken = default);

        Task<TagsModel> GetTransactionTags(string externalToken, long transactionId, CancellationToken cancelToken = default);

        Task AddTransactionTags(string externalToken, long transactionId, UpsertTransactionTagsModel transactionTags, bool? force = default, CancellationToken cancelToken = default);

        Task UpdateTransactionTags(string externalToken, long transactionId, UpsertTransactionTagsModel transactionTags, bool? force = default, CancellationToken cancelToken = default);

        Task<CardholderDetailsModel> GetCardholderDetails(string externalToken, int cardholderAccountId, CancellationToken cancelToken = default);

        Task<CardholderProfileModel> GetCardholderProfile(string externalToken, int cardholderAccountId, CancellationToken cancelToken = default);

        Task UpdateCardholderCardStatus(string externalToken, int cardholderAccountId, CardStatus status, CancellationToken cancelToken = default);

        Task<CardholderGroupsResponseModel> GetCardholderGroups(string externalToken, CancellationToken cancelToken = default);

        Task<CardholderGroupResponseModel> GetCardholderGroup(string externalToken, int groupId, CancellationToken cancelToken = default);

        Task<CardholderGroupResponseModel> CreateCardholderGroup(string externalToken, string groupName, CancellationToken cancelToken = default);

        Task<CardholderGroupResponseModel> UpdateCardholderGroupName(string externalToken, int groupId, string groupName, CancellationToken cancelToken = default);

        Task DeleteCardholderGroup(string externalToken, int groupId, CancellationToken cancelToken = default);

        Task<int> CreateCardOrder(string externalToken, CardOrderModel cardOrder, CancellationToken cancelToken = default);

        Task<FundResponseModel> FundCard(string externalToken, int cardholderAccountId, decimal amount, string note = "", CancellationToken cancelToken = default);

        Task<FundResponseModel> ZeroCard(string externalToken, int cardholderAccountId, CancellationToken cancelToken = default);

        Task<bool> IsTagsEnabled(string externalToken, CancellationToken cancelToken = default);

        Task<bool> IsTagsAvailable(string externalToken, CustomFieldType fieldType, CancellationToken cancelToken = default);

        Task<List<TagDetailsModel>> GetTags(string externalToken, CancellationToken cancelToken = default);

        Task<TagDetailsModel> GetTag(string externalToken, string tagId, CancellationToken cancelToken = default);

        Task<TagDropdownDetailsModel> GetDropdownTag(string externalToken, string tagId, CancellationToken cancelToken = default);

        Task<TagDropdownDetailsModel> GetDropdownTag(string externalToken, string tagId, bool overrideRestrictions, CancellationToken cancelToken = default);

        Task<TagDropdownDetailsModel> CreateDropdownTag(string externalToken, TagDropdownDataModel tag, CancellationToken cancelToken = default);

        Task<TagDropdownDetailsModel> UpdateDropdownTag(string externalToken, string tagId, TagDropdownDetailsModel tag, CancellationToken cancelToken = default);

        Task<TagDropdownDetailsModel> DeleteDropdownTag(string externalToken, string tagId, CancellationToken cancelToken = default);

        Task<List<CallbackSubscriptionModel>> GetCallbackSubscriptions(string externalToken, CallbackType? type = default, CancellationToken cancelToken = default);

        Task<CallbackSubscriptionModel> GetCallbackSubscription(string externalToken, int callbackId, CancellationToken cancelToken = default);

        Task AddCallbackSubscription(string externalToken, CallbackType callbackType, Uri callbackUri, CallbackStatus callbackStatus = CallbackStatus.Active, string name = default, string description = default, CancellationToken cancelToken = default);

        Task UpdateCallbackSubscription(string externalToken, int callbackId, CallbackType callbackType, Uri callbackUri, CallbackStatus callbackStatus, string name = default, string description = default, CancellationToken cancelToken = default);

        Task DeleteCallbackSubscription(string externalToken, int callbackId, CancellationToken cancelToken = default);

        Task<List<InvoiceModel>> GetInvoices(string externalToken, DateTime starDate, CancellationToken cancelToken = default);

        Task<List<InvoiceAllocationModel>> GetInvoiceAllocations(string externalToken, int invoiceId, CancellationToken cancelToken = default);

        Task<List<InvoicePaymentModel>> GetInvoicePayments(string externalToken, int invoiceId, CancellationToken cancelToken = default);

        Task<VendorCardOrderResponseModel> GetVendorCardOrder(string externalToken, int orderId, CancellationToken cancelToken = default);

        Task<VendorCardCreateOrderResponseModel> CreateVendorCardOrder(string externalToken, VendorCardCreateOrderRequestModel createOrder, CancellationToken cancelToken = default);

        Task SendVendorCardData(string externalToken, VendorCardDataModel data, CancellationToken cancelToken = default);

        // Vendor Management
        Task<GetVendorsResponseModel> GetVendors(string externalToken, GetVendorsRequestModel request = null, CancellationToken cancelToken = default);

        Task<VendorModel> GetVendor(string externalToken, int vendorId, CancellationToken cancelToken = default);

        Task<VendorModel> CreateVendor(string externalToken, CreateVendorRequestModel request, CancellationToken cancelToken = default);

        Task<VendorModel> UpdateVendor(string externalToken, int vendorId, UpdateVendorRequestModel request, CancellationToken cancelToken = default);

        Task<VendorModel> UpdateVendorStatus(string externalToken, int vendorId, UpdateVendorStatusRequestModel request, CancellationToken cancelToken = default);

        Task<VendorModel> ApproveVendor(string externalToken, int vendorId, CancellationToken cancelToken = default);

        Task<VendorModel> RejectVendor(string externalToken, int vendorId, CancellationToken cancelToken = default);

        // Vendor Documents
        Task<VendorModel> AddVendorDocument(string externalToken, int vendorId, string fileName, Stream fileContent, CancellationToken cancelToken = default);

        Task<VendorModel> DeleteVendorDocument(string externalToken, int vendorId, int documentId, CancellationToken cancelToken = default);

        // Vendor Cards (linked cardholder accounts)
        Task<VendorModel> AddVendorCard(string externalToken, int vendorId, AddVendorCardRequestModel request, CancellationToken cancelToken = default);

        Task<VendorModel> UpdateVendorCard(string externalToken, int vendorId, int cardholderAcctId, CancellationToken cancelToken = default);

        // Vendor Bank Accounts
        Task<VendorModel> AddVendorBankAccount(string externalToken, int vendorId, AddVendorBankAccountRequestModel request, CancellationToken cancelToken = default);

        Task<VendorModel> UpdateVendorBankAccount(string externalToken, int vendorId, long bankAccountId, UpdateVendorBankAccountRequestModel request, CancellationToken cancelToken = default);

        // Bill Payments
        Task<BillModel> GetBill(string externalToken, int billId, CancellationToken cancelToken = default);

        Task<GetBillPaymentsResponseModel> GetBillPayments(string externalToken, int billId, CancellationToken cancelToken = default);

        Task<BillModel> CreateBill(string externalToken, CreateBillRequestModel request, CancellationToken cancelToken = default);

        Task<SearchBillsResponseModel> SearchBills(string externalToken, SearchBillsRequestModel request = null, CancellationToken cancelToken = default);

        Task UpdateBillTags(string externalToken, int billId, UpdateBillTagsRequestModel request, CancellationToken cancelToken = default);

        Task DeleteBillTags(string externalToken, int billId, CancellationToken cancelToken = default);

        Task<BillNoteResponseModel> AddBillNote(string externalToken, int billId, AddBillNoteRequestModel request, CancellationToken cancelToken = default);

        Task UpdateBillNote(string externalToken, int billId, long noteId, UpdateBillNoteRequestModel request, CancellationToken cancelToken = default);

        Task DeleteBillNote(string externalToken, int billId, long noteId, CancellationToken cancelToken = default);

        Task<UploadBillAttachmentResponseModel> AddBillAttachment(string externalToken, int billId, string fileName, Stream fileContent, CancellationToken cancelToken = default);

        Task<BillModel> SubmitBill(string externalToken, int billId, CancellationToken cancelToken = default);

        Task<BillModel> ApproveBill(string externalToken, int billId, ApproveBillRequestModel request = null, CancellationToken cancelToken = default);

        Task<BillModel> RejectBill(string externalToken, int billId, RejectBillRequestModel request = null, CancellationToken cancelToken = default);

        Task<BillModel> ProcessBill(string externalToken, int billId, ProcessBillRequestModel request = null, CancellationToken cancelToken = default);

        Task<GetSpendingRulesetsResponseModel> GetSpendingRulesets(string externalToken, CancellationToken cancelToken = default);

        Task<GetSpendingRulesetResponseModel> GetSpendingRuleset(string externalToken, int rulesetId, CancellationToken cancelToken = default);

        Task<SpendingRulesetResponseModel> CreateSpendingRuleset(string externalToken, CreateSpendingRulesetRequestModel createRuleset, CancellationToken cancelToken = default);

        Task<SpendingRulesetResponseModel> UpdateSpendingRuleset(string externalToken, UpdateSpendingRulesetRequestModel updateRuleset, CancellationToken cancelToken = default);

        Task<SpendingRulesetResponseModel> DeleteSpendingRuleset(string externalToken, int rulesetId, CancellationToken cancelToken = default);

        Task<List<CardholderDetailsModel>> GetSpendingRulesetCards(string externalToken, int rulesetId, CancellationToken cancelToken = default);

        Task<List<MerchantCategoryModel>> GetMerchantCategories(string externalToken, CancellationToken cancelToken = default);

        Task<StateModel> ApproveTransaction(string externalToken, long transactionId, CancellationToken cancelToken = default);

        Task<StateModel> RejectTransaction(string externalToken, long transactionId, string reason, CancellationToken cancelToken = default);

        Task<StateModel> ResetTransaction(string externalToken, long transactionId, CancellationToken cancelToken = default);

        Task<PaymentListResponseModel> GetPayments(string externalToken, PaymentListRequestModel model, int page = 1, int size = 15, CancellationToken cancelToken = default);

        Task<PaymentTransferModel> GetPaymentTransfer(string externalToken, int paymentTransferId, CancellationToken cancelToken = default);

        Task<PaymentRequestModel> GetPaymentRequest(string externalToken, int paymentRequestId, CancellationToken cancelToken = default);
    }
}
