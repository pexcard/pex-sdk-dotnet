using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using PexCard.Api.Client.Const;
using PexCard.Api.Client.Core;
using PexCard.Api.Client.Core.Enums;
using PexCard.Api.Client.Core.Exceptions;
using PexCard.Api.Client.Core.Models;
using PexCard.Api.Client.Extensions;
using PexCard.Api.Client.Models;

namespace PexCard.Api.Client
{
    public class PexApiClient : IPexApiClient
    {
        private const string PexCorrelationIdHeaderName = "X-CORRELATION-ID";
        private const string PexJsonMediaType = "application/json";
        private readonly Encoding PexEncodingType = Encoding.UTF8;

        private readonly HttpClient _httpClient;

        public PexApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public Uri BaseUri => _httpClient.BaseAddress;

        public async Task<bool> Ping(CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Ping"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return response.IsSuccessStatusCode;
        }

        public async Task<RenewTokenResponseModel> RenewExternalToken(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Token/Renew"));

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<RenewTokenResponseModel>(response);
        }

        public async Task<string> ExchangeJwtForApiToken(string jwt, ExchangeTokenRequestModel exchangeTokenRequest, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "Internal/V4/Account/Token/Exchange"));

            var requestData = exchangeTokenRequest;

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Bearer, jwt);
            request.Content = new StringContent(JsonConvert.SerializeObject(requestData), PexEncodingType, PexJsonMediaType);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<string>(response);
        }

        public async Task DeleteExternalToken(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Token"));

            var request = new HttpRequestMessage(HttpMethod.Delete, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            await HandleHttpResponseMessage(response);
        }

        public async Task<decimal> GetPexAccountBalance(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Business/Balance"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

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
            requestUriQueryParams.Add("StartDate", startDate.ToDateTimeString());
            requestUriQueryParams.Add("EndDate", endDate.ToDateTimeString());
            requestUriBuilder.Query = requestUriQueryParams.ToString();

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<int>(response);
        }

        public async Task<CardholderTransactions> GetAllCardholderTransactions(string externalToken, DateTime startDate, DateTime endDate, bool includePendings = false, bool includeDeclines = false, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Details/AllCardholderTransactions"));

            var requestUriQueryParams = HttpUtility.ParseQueryString(requestUriBuilder.Query);
            requestUriQueryParams.Add("IncludePendings", includePendings.ToString());
            requestUriQueryParams.Add("IncludeDeclines", includeDeclines.ToString());
            requestUriQueryParams.Add("StartDate", startDate.ToDateTimeString());
            requestUriQueryParams.Add("EndDate", endDate.ToDateTimeString());
            requestUriBuilder.Query = requestUriQueryParams.ToString();

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

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
            requestUriQueryParams.Add("StartDate", startDate.ToDateTimeString());
            requestUriQueryParams.Add("EndDate", endDate.ToDateTimeString());
            requestUriBuilder.Query = requestUriQueryParams.ToString();

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            var responseData = await HandleHttpResponseMessage<TransactionListModel>(response);

            return new BusinessAccountTransactions(responseData?.TransactionList ?? new List<TransactionModel>());
        }

        public async Task<List<AttachmentLinkModel>> GetTransactionAttachments(string externalToken, long transactionId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Transactions/{transactionId}/Attachments"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            var responseData = await HandleHttpResponseMessage<AttachmentsModel>(response, true);

            return responseData?.Attachments;
        }

        public async Task<AttachmentModel> GetTransactionAttachment(string externalToken, long transactionId, string attachmentId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Transactions/{transactionId}/Attachment/{attachmentId}"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<AttachmentModel>(response, true);
        }

        public async Task AddTransactionNote(string externalToken, TransactionModel transaction, string noteText, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Note"));

            var requestData = new NoteRequestModel
            {
                NoteText = noteText,
                Pending = transaction.IsPending,
                TransactionId = transaction.TransactionId
            };

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);
            request.Content = new StringContent(JsonConvert.SerializeObject(requestData), PexEncodingType, PexJsonMediaType);

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

        public async Task<GetAdminProfileModel> GetMyAdminProfile(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Business/MyProfile"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<GetAdminProfileModel>(response);
        }

        public async Task<BusinessDetailsModel> GetBusinessDetails(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Details/AccountDetails"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<BusinessDetailsModel>(response);
        }

        public async Task<BusinessSettingsModel> GetBusinessSettings(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Business/Settings"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<BusinessSettingsModel>(response);
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
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);
            request.Content = new StringContent(JsonConvert.SerializeObject(requestData), PexEncodingType, PexJsonMediaType);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<FundResponseModel>(response);
        }

        public async Task<FundResponseModel> ZeroCard(string externalToken, int cardholderAccountId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Card/Zero/{cardholderAccountId}"));

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

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
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            var responseData = await HandleHttpResponseMessage<TransactionListModel>(response);

            return new CardholderTransactions(responseData?.TransactionList ?? new List<TransactionModel>());
        }

        public async Task<CardholderDetailsModel> GetCardholderDetails(string externalToken, int cardholderAccountId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Details/AccountDetails/{cardholderAccountId}"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<CardholderDetailsModel>(response);
        }

        public async Task<CardholderProfileModel> GetCardholderProfile(string externalToken, int cardholderAccountId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Card/Profile/{cardholderAccountId}"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<CardholderProfileModel>(response);
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

        public async Task<TagDropdownDetailsModel> CreateDropdownTag(string externalToken, TagDropdownDataModel tag, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Business/Configuration/Tag/Dropdown"));

            var requestData = tag;

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);
            request.Content = new StringContent(JsonConvert.SerializeObject(requestData), PexEncodingType, PexJsonMediaType);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<TagDropdownDetailsModel>(response);
        }

        public async Task<TagDropdownDetailsModel> UpdateDropdownTag(string externalToken, string tagId, TagDropdownModel tag, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Business/Configuration/Tag/Dropdown/{tagId}"));

            var requestData = tag;

            var request = new HttpRequestMessage(HttpMethod.Put, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);
            request.Content = new StringContent(JsonConvert.SerializeObject(requestData), PexEncodingType, PexJsonMediaType);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<TagDropdownDetailsModel>(response);
        }

        public async Task<TagDropdownDetailsModel> DeleteDropdownTag(string externalToken, string tagId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Business/Configuration/Tag/Dropdown/{tagId}"));

            var request = new HttpRequestMessage(HttpMethod.Delete, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<TagDropdownDetailsModel>(response);
        }

        public async Task<TokenResponseModel> GetTokens(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Token"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<TokenResponseModel>(response);
        }

        public async Task<TokenDataModel> GetToken(string externalToken, CancellationToken cancelToken = default)
        {
            var tokens = await GetTokens(externalToken, cancelToken);

            return tokens.Tokens.FirstOrDefault(pt => pt.Token == externalToken);
        }

        public async Task<int> CreateCardOrder(string externalToken, CardOrderModel cardOrder, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Card/CreateAsync"));

            var requestData = cardOrder;

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);
            request.Content = new StringContent(JsonConvert.SerializeObject(requestData), PexEncodingType, PexJsonMediaType);

            var response = await _httpClient.SendAsync(request, cancelToken);

            var responseData = await HandleHttpResponseMessage<CardOrderIdModel>(response);

            return responseData.CardOrderId;
        }

        public async Task<PartnerModel> GetPartner(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Partner"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<PartnerModel>(response);
        }

        public async Task<CardholderGroupsResponseModel> GetCardholderGroups(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Group"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<CardholderGroupsResponseModel>(response);
        }

        public async Task<CardholderGroupResponseModel> GetCardholderGroup(string externalToken, int groupId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Group/{groupId}"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<CardholderGroupResponseModel>(response);
        }

        public async Task<CardholderGroupResponseModel> CreateCardholderGroup(string externalToken, string groupName, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Group"));

            var requestData = new UpsertCardholderGroupModel { Name = groupName };

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);
            request.Content = new StringContent(JsonConvert.SerializeObject(requestData), PexEncodingType, PexJsonMediaType);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<CardholderGroupResponseModel>(response);
        }

        public async Task<CardholderGroupResponseModel> UpdateCardholderGroupName(string externalToken, int groupId, string groupName, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Group/{groupId}"));

            var requestData = new UpsertCardholderGroupModel { Name = groupName };

            var request = new HttpRequestMessage(HttpMethod.Put, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);
            request.Content = new StringContent(JsonConvert.SerializeObject(requestData), PexEncodingType, PexJsonMediaType);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<CardholderGroupResponseModel>(response);
        }

        public async Task DeleteCardholderGroup(string externalToken, int groupId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Group/{groupId}"));

            var request = new HttpRequestMessage(HttpMethod.Delete, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            await HandleHttpResponseMessage(response);
        }

        public async Task<TagsModel> GetTransactionTags(string externalToken, long transactionId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Transactions/{transactionId}/Tags"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

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
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);
            request.Content = new StringContent(JsonConvert.SerializeObject(requestData), PexEncodingType, PexJsonMediaType);

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
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);
            request.Content = new StringContent(JsonConvert.SerializeObject(requestData), PexEncodingType, PexJsonMediaType);

            var response = await _httpClient.SendAsync(request, cancelToken);

            await HandleHttpResponseMessage(response);
        }

        public async Task<List<CallbackSubscriptionModel>> GetCallbackSubscriptions(string externalToken, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Callback-Subscription"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<List<CallbackSubscriptionModel>>(response);
        }

        public async Task<CallbackSubscriptionModel> GetCallbackSubscription(string externalToken, int callbackId, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Callback-Subscription/{callbackId}"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<CallbackSubscriptionModel>(response);
        }

        public async Task AddCallbackSubscription(string externalToken, CallbackType callbackType, Uri callbackUri, CallbackStatus callbackStatus = CallbackStatus.Active, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Callback-Subscription"));

            var requestData = new UpsertCallbackSubscriptionModel(callbackType, callbackStatus, callbackUri);

            var request = new HttpRequestMessage(HttpMethod.Post, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);
            request.Content = new StringContent(JsonConvert.SerializeObject(requestData), PexEncodingType, PexJsonMediaType);

            var response = await _httpClient.SendAsync(request, cancelToken);

            await HandleHttpResponseMessage(response);
        }

        public async Task UpdateCallbackSubscription(string externalToken, int callbackId, CallbackType callbackType, Uri callbackUri, CallbackStatus callbackStatus, CancellationToken cancelToken = default)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Callback-Subscription/{callbackId}"));

            var requestData = new UpsertCallbackSubscriptionModel(callbackType, callbackStatus, callbackUri);

            var request = new HttpRequestMessage(HttpMethod.Put, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);
            request.Content = new StringContent(JsonConvert.SerializeObject(requestData), PexEncodingType, PexJsonMediaType);

            var response = await _httpClient.SendAsync(request, cancelToken);

            await HandleHttpResponseMessage(response);
        }

        #region Private methods

        private async Task<HttpResponseMessage> GetTagsResponse(string externalToken, CancellationToken cancelToken)
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, "V4/Business/Configuration/Tags"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            return await _httpClient.SendAsync(request, cancelToken);
        }

        private async Task<TTagModel> GetTag<TTagModel>(string externalToken, string tagId, CancellationToken cancelToken) where TTagModel : TagDataModel
        {
            var requestUriBuilder = new UriBuilder(new Uri(BaseUri, $"V4/Business/Configuration/Tag/{tagId}"));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.SendAsync(request, cancelToken);

            return await HandleHttpResponseMessage<TTagModel>(response);
        }

        private async Task<TContent> HandleHttpResponseMessage<TContent>(HttpResponseMessage response, bool notFoundAsDefault = false)
        {
            var responseData = await response.Content.ReadAsStringAsync();
            try
            {
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NotFound && notFoundAsDefault)
                    {
                        return default;
                    }

                    var errorModel = JsonConvert.DeserializeObject<ErrorMessageModel>(responseData);
                    var correlationId = GetPexCorrelationId(response);

                    throw new PexApiClientException(response.StatusCode, errorModel.Message, correlationId);
                }
                return JsonConvert.DeserializeObject<TContent>(responseData);
            }
            catch (Exception ex)
            {
                var correlationId = GetPexCorrelationId(response);

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
                    var correlationId = GetPexCorrelationId(response);

                    throw new PexApiClientException(response.StatusCode, errorModel.Message, correlationId);
                }
                catch (Exception ex)
                {
                    var correlationId = GetPexCorrelationId(response);

                    throw new PexApiClientException(response.StatusCode, $"Error parsing response: {ex.Message}\nContent: {responseData}", ex, correlationId);
                }
            }
        }

        private string GetPexCorrelationId(HttpResponseMessage response)
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            string correlationId = default;

            if (response.Headers.TryGetValues(PexCorrelationIdHeaderName, out var correlationIdHeaders))
            {
                correlationId = correlationIdHeaders.FirstOrDefault();
            }

            return correlationId;
        }

        #endregion
    }
}
