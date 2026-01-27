using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using PexCard.Api.Client.Core;
using PexCard.Api.Client.Core.Enums;
using PexCard.Api.Client.Core.Exceptions;
using PexCard.Api.Client.Core.Interfaces;
using PexCard.Api.Client.Core.Models;
using PexCard.Api.Client.Extensions;
using PexCard.Api.Client.Models;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PexCard.Api.Client
{
    public class PexApiClient : IPexApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IIPAddressResolver _ipAddressResolver;
        private readonly ICorrelationIdResolver _correlationIdResolver;

        public PexApiClient(HttpClient httpClient,
                            IIPAddressResolver ipAddressResolver = null,
                            ICorrelationIdResolver correlationIdResolver = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _ipAddressResolver = ipAddressResolver ?? new DummyIpAddressResolver();
            _correlationIdResolver = correlationIdResolver ?? new DefaultCorrelationIdResolver();
        }

        public Uri BaseUri => _httpClient.BaseAddress;

        public async Task<bool> Ping(CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Ping"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return response.IsSuccessStatusCode;
        }

        public async Task<string> ExchangeJwtForApiToken(string jwt, ExchangeTokenRequestModel exchangeTokenRequest, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "Internal/V4/Account/Token/Exchange"));

            var requestData = exchangeTokenRequest;

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationBearerHeader(jwt);
            request.SetPexJsonContent(requestData);
            request.SetXForwardForHeader(_ipAddressResolver.GetValue());


            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<string>(response);
        }

        public async Task<TokenDataModel> GetToken(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Token/Current"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetXForwardForHeader(_ipAddressResolver.GetValue());

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<TokenDataModel>(response);
        }

        public async Task<TokenResponseModel> GetTokens(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Token"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);

            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetXForwardForHeader(_ipAddressResolver.GetValue());

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<TokenResponseModel>(response);
        }

        public async Task<RenewTokenResponseModel> RenewExternalToken(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Token/Renew"));

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetXForwardForHeader(_ipAddressResolver.GetValue());

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<RenewTokenResponseModel>(response);
        }

        public async Task DeleteExternalToken(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Token"));

            var request = new HttpRequestMessage(HttpMethod.Delete, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetXForwardForHeader(_ipAddressResolver.GetValue());

            var response = await _httpClient.SendAsync(request, cancelToken);

            await HandleHttpResponseMessage(response);
        }

        public async Task<PartnerModel> GetPartner(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Partner"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<PartnerModel>(response);
        }


        public async Task<decimal> GetPexAccountBalance(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Business/Balance"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            var responseData = await HandleHttpResponseMessage<BusinessBalanceModel>(response);

            return responseData?.BusinessAccountBalance ?? 0;
        }

        public async Task<int> GetAllCardholderTransactionsCount(string externalToken, DateTime startDate, DateTime endDate, bool includePendings = false, bool includeDeclines = false, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Details/AllCardholderTransactionCount"));

            var requestUriQueryParams = HttpUtility.ParseQueryString(requestUriBuilder.Query);
            requestUriQueryParams.Add("IncludePendings", includePendings.ToString());
            requestUriQueryParams.Add("IncludeDeclines", includeDeclines.ToString());
            requestUriQueryParams.Add("StartDate", startDate.ToEst().ToDateTimeString());
            requestUriQueryParams.Add("EndDate", endDate.ToEst().ToDateTimeString());
            requestUriBuilder.Query = requestUriQueryParams.ToString();

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<int>(response);
        }

        public async Task<CardholderTransactions> GetAllCardholderTransactions(string externalToken, DateTime startDate, DateTime endDate, bool includePendings = false, bool includeDeclines = false, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Details/AllCardholderTransactions"));

            var requestUriQueryParams = HttpUtility.ParseQueryString(requestUriBuilder.Query);
            requestUriQueryParams.Add("IncludePendings", includePendings.ToString());
            requestUriQueryParams.Add("IncludeDeclines", includeDeclines.ToString());
            requestUriQueryParams.Add("StartDate", startDate.ToEst().ToDateTimeString());
            requestUriQueryParams.Add("EndDate", endDate.ToEst().ToDateTimeString());
            requestUriBuilder.Query = requestUriQueryParams.ToString();

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            var responseData = await HandleHttpResponseMessage<TransactionListModel>(response);

            return new CardholderTransactions(responseData?.TransactionList ?? new List<TransactionModel>());
        }

        public async Task<BusinessAccountTransactions> GetBusinessAccountTransactions(string externalToken, DateTime startDate, DateTime endDate, bool includePendings = false, bool includeDeclines = false, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Details/TransactionDetails"));

            var requestUriQueryParams = HttpUtility.ParseQueryString(requestUriBuilder.Query);
            requestUriQueryParams.Add("IncludePendings", includePendings.ToString());
            requestUriQueryParams.Add("IncludeDeclines", includeDeclines.ToString());
            requestUriQueryParams.Add("StartDate", startDate.ToEst().ToDateTimeString());
            requestUriQueryParams.Add("EndDate", endDate.ToEst().ToDateTimeString());
            requestUriBuilder.Query = requestUriQueryParams.ToString();

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            var responseData = await HandleHttpResponseMessage<TransactionListModel>(response);

            return new BusinessAccountTransactions(responseData?.TransactionList ?? new List<TransactionModel>());
        }

        public async Task<List<AttachmentLinkModel>> GetTransactionAttachments(string externalToken, long transactionId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Transactions/{transactionId}/Attachments"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            var responseData = await HandleHttpResponseMessage<AttachmentsModel>(response, returnValueForNotFound: true);

            return responseData?.Attachments ?? new List<AttachmentLinkModel>();
        }

        public async Task<AttachmentModel> GetTransactionAttachment(string externalToken, long transactionId, string attachmentId, AttachmentLinkType attachmentLinkType = AttachmentLinkType.LinkUrl, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Transactions/{transactionId}/Attachment/{attachmentId}"));

            var requestUriQueryParams = HttpUtility.ParseQueryString(requestUriBuilder.Query);
            requestUriQueryParams.Add("AttachmentLinkType", attachmentLinkType.ToString());
            requestUriBuilder.Query = requestUriQueryParams.ToString();

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<AttachmentModel>(response, returnValueForNotFound: true);
        }

        public async Task AddTransactionNote(string externalToken, TransactionModel transaction, string noteText, bool visibleToCardholder = false, bool systemGenerated = true, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Note"));

            var requestData = new NoteRequestModel
            {
                NoteText = noteText,
                Pending = transaction.IsPending,
                TransactionId = transaction.TransactionId,
                VisibleToCardholder = visibleToCardholder,
                SystemGenerated = systemGenerated
            };

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            await HandleHttpResponseMessage(response);
        }

        public async Task AddTransactionRelationshipNote(string externalToken, long transactionRelationshipId, string noteText, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Note/TransactionRelationshipNote"));

            var requestData = new
            {
                NoteText = noteText,
                TransactionRelationshipId = transactionRelationshipId
            };

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            await HandleHttpResponseMessage(response);
        }

        public async Task UpdateTransactionNote(string externalToken, long noteId, string noteText, bool isPending, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Note/{noteId}"));

            var requestData = new NoteRequestModel
            {
                NoteText = noteText,
                Pending = isPending
            };

            var request = new HttpRequestMessage(HttpMethod.Put, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            await HandleHttpResponseMessage(response);
        }

        public async Task DeleteTransactionNote(string externalToken, long noteId, bool isPending, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Note/{noteId}"));

            var requestData = new NoteRequestModel
            {
                Pending = isPending
            };

            var request = new HttpRequestMessage(HttpMethod.Delete, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            await HandleHttpResponseMessage(response);
        }

        public async Task<bool> IsTagsEnabled(string externalToken, CancellationToken cancelToken = default)
        {
            var response = await GetTagsResponse(externalToken, cancelToken);

            if (response.StatusCode == HttpStatusCode.Forbidden) return false;

            await HandleHttpResponseMessage(response);

            return true;
        }

        public async Task<bool> IsTagsAvailable(string externalToken, CustomFieldType fieldType, CancellationToken cancelToken = default)
        {
            var response = await GetTagsResponse(externalToken, cancelToken);

            if (response.StatusCode == HttpStatusCode.Forbidden) return false;

            var responseData = await HandleHttpResponseMessage<List<TagDetailsModel>>(response);

            return responseData.Any(x => x.Type == fieldType);
        }

        public async Task<BusinessDetailsModel> GetBusinessDetails(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Details/AccountDetails"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<BusinessDetailsModel>(response);
        }

        public async Task<BusinessProfileModel> GetBusinessProfile(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Business/Profile"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<BusinessProfileModel>(response);
        }

        public async Task<BusinessSettingsModel> GetBusinessSettings(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Business/Settings"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<BusinessSettingsModel>(response);
        }

        public async Task<List<LinkedBusinessModel>> GetLinkedBusinesses(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Business/Linked"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<List<LinkedBusinessModel>>(response);
        }

        public async Task<BusinessAdminReponseModel> GetMyAdminProfile(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Business/MyProfile"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<BusinessAdminReponseModel>(response);
        }

        public async Task<BusinessAdminsReponseModel> GetBusinessAdmins(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Business/Admin"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<BusinessAdminsReponseModel>(response);
        }

        public async Task<BusinessAdminReponseModel> GetBusinessAdmin(string externalToken, long adminId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Business/Admin/{adminId}"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<BusinessAdminReponseModel>(response);
        }

        public async Task<BusinessAdminReponseModel> CreateBusinessAdmin(string externalToken, CreateBusinessAdminModel newAdmin, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Business/Admin"));

            var requestData = newAdmin;

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<BusinessAdminReponseModel>(response);
        }

        public async Task<BusinessAdminReponseModel> UpdateBusinessAdmin(string externalToken, long adminId, UpdateBusinessAdminModel updateAdmin, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Business/Admin/{adminId}"));

            var requestData = updateAdmin;

            var request = new HttpRequestMessage(HttpMethod.Put, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<BusinessAdminReponseModel>(response);
        }

        public async Task DeleteBusinessAdmin(string externalToken, long adminId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Business/Admin/{adminId}"));

            var request = new HttpRequestMessage(HttpMethod.Delete, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            await HandleHttpResponseMessage(response);
        }

        public async Task<FundResponseModel> FundCard(string externalToken, int cardholderAccountId, decimal amount, string note = "", CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Card/Fund/{cardholderAccountId}"));

            var requestData = new FundRequestModel
            {
                Amount = amount,
                NoteText = note
            };

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.DontRetryRequest();
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<FundResponseModel>(response);
        }

        public async Task<FundResponseModel> ZeroCard(string externalToken, int cardholderAccountId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Card/Zero/{cardholderAccountId}"));

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<FundResponseModel>(response);
        }

        public async Task<CardholderTransactions> GetCardholderTransactions(string externalToken, int cardholderAccountId, DateTime startDate, DateTime endDate, bool includePending = false, bool includeDeclines = false, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Details/TransactionDetails/{cardholderAccountId}"));

            var requestUriQueryParams = HttpUtility.ParseQueryString(requestUriBuilder.Query);
            requestUriQueryParams.Add("IncludePendings", includePending.ToString());
            requestUriQueryParams.Add("IncludeDeclines", includeDeclines.ToString());
            requestUriQueryParams.Add("StartDate", startDate.ToEst().ToDateTimeString());
            requestUriQueryParams.Add("EndDate", endDate.ToEst().ToDateTimeString());
            requestUriBuilder.Query = requestUriQueryParams.ToString();

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            var responseData = await HandleHttpResponseMessage<TransactionListModel>(response);

            return new CardholderTransactions(responseData?.TransactionList ?? new List<TransactionModel>());
        }

        public async Task<CardholderDetailsModel> GetCardholderDetails(string externalToken, int cardholderAccountId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Details/AccountDetails/{cardholderAccountId}"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<CardholderDetailsModel>(response);
        }

        public async Task<CardholderProfileModel> GetCardholderProfile(string externalToken, int cardholderAccountId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Card/Profile/{cardholderAccountId}"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<CardholderProfileModel>(response);
        }

        public async Task UpdateCardholderCardStatus(string externalToken, int cardholderAccountId, CardStatus status, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Card/Status/{cardholderAccountId}"));

            var requestData = new UpdateCardStatusRequestModel
            {
                Status = status
            };

            var request = new HttpRequestMessage(HttpMethod.Put, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            await HandleHttpResponseMessage(response);
        }

        public async Task<List<TagDetailsModel>> GetTags(string externalToken, CancellationToken cancelToken = default)
        {
            var response = await GetTagsResponse(externalToken, cancelToken);

            return await HandleHttpResponseMessage<List<TagDetailsModel>>(response);
        }

        public async Task<TagDetailsModel> GetTag(string externalToken, string tagId, CancellationToken cancelToken = default)
        {
            return await GetTag<TagDetailsModel>(externalToken, tagId, cancelToken);
        }

        public async Task<TagDropdownDetailsModel> GetDropdownTag(string externalToken, string tagId, CancellationToken cancelToken = default)
        {
            return await GetTag<TagDropdownDetailsModel>(externalToken, tagId, cancelToken);
        }

        public async Task<TagDropdownDetailsModel> GetDropdownTag(string externalToken, string tagId, bool overrideRestrictions, CancellationToken cancelToken = default)
        {
            return await GetTag<TagDropdownDetailsModel>(externalToken, tagId, overrideRestrictions, cancelToken);
        }

        public async Task<TagDropdownDetailsModel> CreateDropdownTag(string externalToken, TagDropdownDataModel tag, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Business/Configuration/Tag/Dropdown"));

            var requestData = tag;

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<TagDropdownDetailsModel>(response);
        }

        public async Task<TagDropdownDetailsModel> UpdateDropdownTag(string externalToken, string tagId, TagDropdownDetailsModel tag, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Business/Configuration/Tag/Dropdown/{tagId}"));

            var requestData = tag;

            var request = new HttpRequestMessage(HttpMethod.Put, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<TagDropdownDetailsModel>(response);
        }

        public async Task<TagDropdownDetailsModel> DeleteDropdownTag(string externalToken, string tagId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Business/Configuration/Tag/Dropdown/{tagId}"));

            var request = new HttpRequestMessage(HttpMethod.Delete, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<TagDropdownDetailsModel>(response);
        }

        public async Task<int> CreateCardOrder(string externalToken, CardOrderModel cardOrder, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Card/CreateAsync"));

            var requestData = cardOrder;

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            var responseData = await HandleHttpResponseMessage<CardOrderIdModel>(response);

            return responseData.CardOrderId;
        }

        public async Task<CardholderGroupsResponseModel> GetCardholderGroups(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Group"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<CardholderGroupsResponseModel>(response);
        }

        public async Task<CardholderGroupResponseModel> GetCardholderGroup(string externalToken, int groupId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Group/{groupId}"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<CardholderGroupResponseModel>(response);
        }

        public async Task<CardholderGroupResponseModel> CreateCardholderGroup(string externalToken, string groupName, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Group"));

            var requestData = new UpsertCardholderGroupModel { Name = groupName };

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<CardholderGroupResponseModel>(response);
        }

        public async Task<CardholderGroupResponseModel> UpdateCardholderGroupName(string externalToken, int groupId, string groupName, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Group/{groupId}"));

            var requestData = new UpsertCardholderGroupModel { Name = groupName };

            var request = new HttpRequestMessage(HttpMethod.Put, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<CardholderGroupResponseModel>(response);
        }

        public async Task DeleteCardholderGroup(string externalToken, int groupId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Group/{groupId}"));

            var request = new HttpRequestMessage(HttpMethod.Delete, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            await HandleHttpResponseMessage(response);
        }

        public async Task<TagsModel> GetTransactionTags(string externalToken, long transactionId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Transactions/{transactionId}/Tags"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<TagsModel>(response);
        }

        public async Task AddTransactionTags(string externalToken, long transactionId, UpsertTransactionTagsModel transactionTags, bool? force = default, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Transactions/{transactionId}/Tags"));

            var requestUriQueryParams = HttpUtility.ParseQueryString(requestUriBuilder.Query);
            if (force.HasValue)
            {
                requestUriQueryParams.Add("force", force.ToString());
            }
            requestUriBuilder.Query = requestUriQueryParams.ToString();

            var requestData = transactionTags;

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            await HandleHttpResponseMessage(response);
        }

        public async Task UpdateTransactionTags(string externalToken, long transactionId, UpsertTransactionTagsModel transactionTags, bool? force = default, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Transactions/{transactionId}/Tags"));

            var requestUriQueryParams = HttpUtility.ParseQueryString(requestUriBuilder.Query);
            if (force.HasValue)
            {
                requestUriQueryParams.Add("force", force.ToString());
            }
            requestUriBuilder.Query = requestUriQueryParams.ToString();

            var requestData = transactionTags;

            var request = new HttpRequestMessage(HttpMethod.Put, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            await HandleHttpResponseMessage(response);
        }

        public async Task<List<CallbackSubscriptionModel>> GetCallbackSubscriptions(string externalToken, CallbackType? type = default, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Callback-Subscription"));

            var requestUriQueryParams = HttpUtility.ParseQueryString(requestUriBuilder.Query);
            if (type.HasValue)
            {
                requestUriQueryParams.Add("type", type.ToString());
            }
            requestUriBuilder.Query = requestUriQueryParams.ToString();

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<List<CallbackSubscriptionModel>>(response);
        }

        public async Task<CallbackSubscriptionModel> GetCallbackSubscription(string externalToken, int callbackId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Callback-Subscription/{callbackId}"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<CallbackSubscriptionModel>(response);
        }

        public async Task AddCallbackSubscription(string externalToken, CallbackType callbackType, Uri callbackUri, CallbackStatus callbackStatus = CallbackStatus.Active, string name = default, string description = default, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Callback-Subscription"));

            var requestData = new UpsertCallbackSubscriptionModel(callbackType, callbackStatus, callbackUri, name, description);

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            await HandleHttpResponseMessage(response);
        }

        public async Task UpdateCallbackSubscription(string externalToken, int callbackId, CallbackType callbackType, Uri callbackUri, CallbackStatus callbackStatus, string name = default, string description = default, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Callback-Subscription/{callbackId}"));

            var requestData = new UpsertCallbackSubscriptionModel(callbackType, callbackStatus, callbackUri, name, description);

            var request = new HttpRequestMessage(HttpMethod.Put, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            await HandleHttpResponseMessage(response);
        }

        public async Task DeleteCallbackSubscription(string externalToken, int callbackId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Callback-Subscription/{callbackId}"));

            var request = new HttpRequestMessage(HttpMethod.Delete, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            await HandleHttpResponseMessage(response);
        }

        public async Task<List<InvoiceModel>> GetInvoices(string externalToken, DateTime starDate, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Invoices?startDate={starDate}"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<List<InvoiceModel>>(response);
        }

        public async Task<List<InvoiceAllocationModel>> GetInvoiceAllocations(string externalToken, int invoiceId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Invoice/{invoiceId}/allocations"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<List<InvoiceAllocationModel>>(response);
        }

        public async Task<List<InvoicePaymentModel>> GetInvoicePayments(string externalToken, int invoiceId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Invoice/{invoiceId}/payments"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<List<InvoicePaymentModel>>(response);
        }

        public async Task<VendorCardOrderResponseModel> GetVendorCardOrder(string externalToken, int orderId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/VendorCard/Order/{orderId}"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<VendorCardOrderResponseModel>(response);
        }

        public async Task<VendorCardCreateOrderResponseModel> CreateVendorCardOrder(string externalToken, VendorCardCreateOrderRequestModel createOrder, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/VendorCard/Order"));

            var requestData = createOrder;

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<VendorCardCreateOrderResponseModel>(response);
        }

        public async Task SendVendorCardData(string externalToken, VendorCardDataModel data, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/VendorCard/Data"));

            var requestData = data;

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            await HandleHttpResponseMessage(response);
        }

        public async Task<GetSpendingRulesetsResponseModel> GetSpendingRulesets(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/SpendingRuleset"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<GetSpendingRulesetsResponseModel>(response);
        }

        public async Task<GetSpendingRulesetResponseModel> GetSpendingRuleset(string externalToken, int rulesetId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/SpendingRuleset/{rulesetId}"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<GetSpendingRulesetResponseModel>(response);
        }

        public async Task<SpendingRulesetResponseModel> CreateSpendingRuleset(string externalToken, CreateSpendingRulesetRequestModel createRuleset, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/SpendingRuleset"));

            var requestData = createRuleset;

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<SpendingRulesetResponseModel>(response);
        }

        public async Task<SpendingRulesetResponseModel> UpdateSpendingRuleset(string externalToken, UpdateSpendingRulesetRequestModel updateRuleset, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/SpendingRuleset"));

            var requestData = updateRuleset;

            var request = new HttpRequestMessage(HttpMethod.Put, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<SpendingRulesetResponseModel>(response);
        }

        public async Task<SpendingRulesetResponseModel> DeleteSpendingRuleset(string externalToken, int rulesetId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/SpendingRuleset"));

            var requestData = new DeleteSpendingRulesetRequestModel
            {
                RulesetId = rulesetId
            };

            var request = new HttpRequestMessage(HttpMethod.Delete, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<SpendingRulesetResponseModel>(response);
        }

        public async Task<List<CardholderDetailsModel>> GetSpendingRulesetCards(string externalToken, int rulesetId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/SpendingRuleset/{rulesetId}/Cards"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<List<CardholderDetailsModel>>(response);
        }

        public async Task<List<MerchantCategoryModel>> GetMerchantCategories(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/SpendingRuleset/MccCategories"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<List<MerchantCategoryModel>>(response);
        }

        public async Task<StateModel> ApproveTransaction(string externalToken, long transactionId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Approval/{transactionId}/Approve"));

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<StateModel>(response);
        }

        public async Task<StateModel> RejectTransaction(string externalToken, long transactionId, string reason, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Approval/{transactionId}/Reject"));

            var requestData = new RejectRequestModel { Reason = reason };

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);
            request.SetPexJsonContent(requestData);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<StateModel>(response);
        }

        public async Task<StateModel> ResetTransaction(string externalToken, long transactionId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Approval/{transactionId}/Reset"));

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<StateModel>(response);
        }

        public async Task<PaymentListResponseModel> GetPayments(string externalToken, PaymentListRequestModel model, int page = 1, int size = 15, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Payments"));

            var requestUriQueryParams = HttpUtility.ParseQueryString(requestUriBuilder.Query);

            if (model != null)
            {
                if (model.PaymentStatuses != null && model.PaymentStatuses.Any())
                {
                    foreach (var paymentStatus in model.PaymentStatuses)
                    {
                        requestUriQueryParams.Add("PaymentStatuses", paymentStatus.ToString());
                    }
                }
                if (model.PaymentStatusTriggers != null && model.PaymentStatusTriggers.Any())
                {
                    foreach (var paymentStatusTrigger in model.PaymentStatusTriggers)
                    {
                        requestUriQueryParams.Add("PaymentStatusTriggers", paymentStatusTrigger.ToString());
                    }
                }
                if (model.OutboundAchCreationStartDate.HasValue)
                {
                    requestUriQueryParams.Add("OutboundAchCreationStartDate", model.OutboundAchCreationStartDate.Value.ToEst().ToDateTimeString());
                }
                if (model.OutboundAchCreationEndDate.HasValue)
                {
                    requestUriQueryParams.Add("OutboundAchCreationEndDate", model.OutboundAchCreationEndDate.Value.ToEst().ToDateTimeString());
                }
                if (model.ExpectedPaymentStartDate.HasValue)
                {
                    requestUriQueryParams.Add("ExpectedPaymentStartDate", model.ExpectedPaymentStartDate.Value.ToEst().ToDateTimeString());
                }
                if (model.ExpectedPaymentEndDate.HasValue)
                {
                    requestUriQueryParams.Add("ExpectedPaymentEndDate", model.ExpectedPaymentEndDate.Value.ToEst().ToDateTimeString());
                }

                requestUriQueryParams.Add("Page", page.ToString());
                requestUriQueryParams.Add("Size", size.ToString());
                requestUriBuilder.Query = requestUriQueryParams.ToString();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);

            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<PaymentListResponseModel>(response);
        }

        public async Task<PaymentTransferModel> GetPaymentTransfer(string externalToken, int paymentTransferId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/PaymentTransfers/{paymentTransferId}"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<PaymentTransferModel>(response);
        }

        public async Task<PaymentRequestModel> GetPaymentRequest(string externalToken, int paymentRequestId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/PaymentRequests/{paymentRequestId}"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<PaymentRequestModel>(response);
        }

        public async Task<BillPaymentRequestModel> GetBillPaymentRequest(string externalToken, int billPaymentRequestId, bool includeAttachmentContent, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/BillPaymentRequest/{billPaymentRequestId}"));

            var requestUriQueryParams = HttpUtility.ParseQueryString(requestUriBuilder.Query);

            requestUriQueryParams.Add("IncludeAttachmentContent", includeAttachmentContent.ToString());
            requestUriBuilder.Query = requestUriQueryParams.ToString();

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<BillPaymentRequestModel>(response);
        }

        public async Task<BillPaymentListResponseModel> GetBillPayments(string externalToken, BillPaymentListRequestModel model, int page = 1, int pageSize = 15, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Bill"));

            var requestUriQueryParams = HttpUtility.ParseQueryString(requestUriBuilder.Query);

            if (model != null)
            {
                if (model.CreatedDateFrom.HasValue)
                {
                    requestUriQueryParams.Add("CreatedDateFrom", model.CreatedDateFrom.Value.ToEst().ToDateTimeString());
                }
                if (model.CreatedDateTo.HasValue)
                {
                    requestUriQueryParams.Add("CreatedDateTo", model.CreatedDateTo.Value.ToEst().ToDateTimeString());
                }
                if (model.DueDateFrom.HasValue)
                {
                    requestUriQueryParams.Add("DueDateFrom", model.DueDateFrom.Value.ToEst().ToDateTimeString());
                }
                if (model.DueDateTo.HasValue)
                {
                    requestUriQueryParams.Add("DueDateTo", model.DueDateTo.Value.ToEst().ToDateTimeString());
                }
                if (model.CreatedByUserId.HasValue)
                {
                    requestUriQueryParams.Add("CreatedByUserId", model.CreatedByUserId.Value.ToString());
                }
                if (model.VendorId.HasValue)
                {
                    requestUriQueryParams.Add("VendorId", model.VendorId.Value.ToString());
                }
                if (model.AmountFrom.HasValue)
                {
                    requestUriQueryParams.Add("AmountFrom", model.AmountFrom.Value.ToString());
                }
                if (model.AmountTo.HasValue)
                {
                    requestUriQueryParams.Add("AmountTo", model.AmountTo.Value.ToString());
                }
                if (model.PaymentRequestStatuses != null && model.PaymentRequestStatuses.Any())
                {
                    foreach (var status in model.PaymentRequestStatuses)
                    {
                        requestUriQueryParams.Add("PaymentRequestStatuses", status.ToString());
                    }
                }
            }

            requestUriQueryParams.Add("Page", page.ToString());
            requestUriQueryParams.Add("PageSize", pageSize.ToString());
            requestUriBuilder.Query = requestUriQueryParams.ToString();

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<BillPaymentListResponseModel>(response);
        }

        #region Private methods

        private async Task<HttpResponseMessage> GetTagsResponse(string externalToken, CancellationToken cancelToken)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Business/Configuration/Tags"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            return await _httpClient.SendAsync(request, cancelToken);
        }

        private async Task<TTagModel> GetTag<TTagModel>(string externalToken, string tagId, CancellationToken cancelToken) where TTagModel : TagDataModel
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Business/Configuration/Tag/{tagId}"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<TTagModel>(response);
        }

        private async Task<TTagModel> GetTag<TTagModel>(string externalToken, string tagId, bool overrideRestrictions, CancellationToken cancelToken) where TTagModel : TagDataModel
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Business/Configuration/Tag/{tagId}?OverrideRestrictions={overrideRestrictions.ToString().ToLower()}"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.SetPexCorrelationIdHeader(_correlationIdResolver.GetValue());
            request.SetPexAcceptJsonHeader();
            request.SetPexAuthorizationTokenHeader(externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<TTagModel>(response);
        }

        private async Task<TData> HandleHttpResponseMessage<TData>(HttpResponseMessage response, bool returnValueForNotFound = false, TData notFoundValue = default)
        {
            var responseData = await response.Content.ReadAsStringAsync();

            try
            {
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NotFound && returnValueForNotFound)
                    {
                        return notFoundValue;
                    }

                    var correlationId = response.GetPexCorrelationId();

                    if (response.IsPexJsonContent())
                    {
                        var errorModel = JsonConvert.DeserializeObject<ErrorMessageModel>(responseData);

                        throw new PexApiClientException(response.StatusCode, errorModel?.Message ?? response.ReasonPhrase, correlationId);
                    }
                    else
                    {
                        throw new PexApiClientException(response.StatusCode, response.ReasonPhrase ?? $"Error {response.StatusCode}", correlationId);
                    }
                }

                return responseData.FromPexJson<TData>();
            }
            catch (PexApiClientException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var correlationId = response.GetPexCorrelationId();

                throw new PexApiClientException(response.StatusCode, $"Error parsing response: {ex.Message}\nContent: {responseData}", ex, correlationId);
            }
        }

        private async Task HandleHttpResponseMessage(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();

                try
                {
                    var errorModel = JsonConvert.DeserializeObject<ErrorMessageModel>(responseData);
                    var correlationId = response.GetPexCorrelationId();

                    throw new PexApiClientException(response.StatusCode, errorModel.Message, correlationId);
                }
                catch (PexApiClientException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    var correlationId = response.GetPexCorrelationId();

                    throw new PexApiClientException(response.StatusCode, $"Error parsing response: {ex.Message}\nContent: {responseData}", ex, correlationId);
                }
            }
        }

        #endregion
    }
}
