using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PexCard.Api.Client.Core.Enums;
using PexCard.Api.Client.Core.Models;

namespace PexCard.Api.Client.Core
{
    public interface IPexApiClient
    {
        Uri BaseUri { get; }
        Task<bool> Ping(CancellationToken token = default(CancellationToken));

        Task<RenewTokenResponseModel> RenewExternalToken(string externalToken, CancellationToken token = default(CancellationToken));
        Task DeleteExternalToken(string externalToken, CancellationToken token = default(CancellationToken));
        Task<decimal> GetPexAccountBalance(string externalToken, CancellationToken token = default(CancellationToken));

        Task<int> GetAllCardholderTransactionsCount(
            string externalToken,
            DateTime startDate,
            DateTime endDate,
            bool includePendings = false,
            bool includeDeclines = false,
            CancellationToken token = default(CancellationToken));

        Task<CardholderTransactions> GetAllCardholderTransactions(
            string externalToken,
            DateTime startDate,
            DateTime endDate,
            bool includePendings = false,
            bool includeDeclines = false,
            CancellationToken token = default(CancellationToken));

        Task<BusinessAccountTransactions> GetBusinessAccountTransactions(
            string externalToken,
            DateTime startDate,
            DateTime endDate,
            bool includePendings = false,
            bool includeDeclines = false,
            CancellationToken token = default(CancellationToken));

        Task<List<AttachmentLinkModel>> GetTransactionAttachments(string externalToken, long transactionId,
            CancellationToken token = default(CancellationToken));

        Task<AttachmentModel> GetTransactionAttachment(string externalToken, long transactionId,
            string attachmentId, CancellationToken token = default(CancellationToken));

        Task AddTransactionNote(string externalToken, TransactionModel transaction, string noteText,
            CancellationToken token = default(CancellationToken));

        Task<bool> IsTagsEnabled(string externalToken, CancellationToken token = default(CancellationToken));
        Task<bool> IsTagsAvailable(string externalToken, CustomFieldType fieldType, CancellationToken token = default(CancellationToken));

        Task<GetAdminProfileModel> GetMyAdminProfile(string externalToken, CancellationToken token = default(CancellationToken));
        Task<BusinessDetailsModel> GetBusinessDetails(string externalToken, CancellationToken token = default(CancellationToken));
        Task<BusinessSettingsModel> GetBusinessSettings(string externalToken, CancellationToken token = default(CancellationToken));

        Task<CardholderDetailsModel> GetCardholderDetails(string externalToken, int cardholderAccountId,
            CancellationToken token = default(CancellationToken));

        Task<CardholderProfileModel> GetCardholderProfile(string externalToken, int cardholderAccountId,
            CancellationToken token = default(CancellationToken));

        Task<List<TagDetailsModel>> GetTags(string externalToken, CancellationToken token = default(CancellationToken));
        Task<TagDetailsModel> GetTag(string externalToken, string tagId, CancellationToken token = default(CancellationToken));
        Task<TagDropdownDetailsModel> GetDropdownTag(string externalToken, string tagId,
            CancellationToken token = default(CancellationToken));
        Task<TagDropdownDetailsModel> CreateDropdownTag(string externalToken, TagDropdownDataModel tag,
            CancellationToken token = default(CancellationToken));
        Task<TagDropdownDetailsModel> UpdateDropdownTag(string externalToken, string tagId, TagDropdownModel tag,
            CancellationToken token = default(CancellationToken));
        Task<TagDropdownDetailsModel> DeleteDropdownTag(string externalToken, string tagId,
            CancellationToken token = default(CancellationToken));

        Task<string> ExchangeJwtForApiToken(string jwt, ExchangeTokenRequestModel exchangeTokenRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<FundResponseModel> FundCard(string externalToken, int cardholderAccountId, decimal amount,
            string note = "", CancellationToken token = default(CancellationToken));

        Task<CardholderTransactions> GetCardholderTransactions(
            string externalToken,
            int cardholderAccountId,
            DateTime startDate,
            DateTime endDate,
            bool includePending = false,
            bool includeDeclines = false,
            CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Returns a list of tokens for the associated business.
        /// Determine which API user a token belongs to, as well as the token expiration date.
        /// </summary>
        Task<TokenResponseModel> GetTokens(string externalToken, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Fund a specified card accountID to zero ($0).
        /// </summary>
        Task<FundResponseModel> ZeroCard(string externalToken, int cardholderAccountId, CancellationToken token = default);

        /// <summary>
        /// Create a new card order. This will create new cardholder accounts within your PEX account.
        /// </summary>
        Task<int> CreateCardOrder(string externalToken, CardOrderModel cardOrder,
            CancellationToken token = default(CancellationToken));

        Task<PartnerModel> GetPartner(string externalToken, CancellationToken token = default(CancellationToken));
        Task<TokenDataModel> GetToken(string externalToken, CancellationToken cancellationToken = default);

        Task<CardholderGroupsResponseModel> GetCardholderGroups(string externalToken, CancellationToken token = default);

        Task<CardholderGroupResponseModel> GetCardholderGroup(string externalToken, int groupId, CancellationToken token = default);

        Task<CardholderGroupResponseModel> CreateCardholderGroup(string externalToken, string groupName, CancellationToken token = default);

        Task<CardholderGroupResponseModel> UpdateCardholderGroupName(string externalToken, int groupId, string groupName, CancellationToken token = default);

        Task DeleteCardholderGroup(string externalToken, int groupId, CancellationToken token = default);
    }
}
