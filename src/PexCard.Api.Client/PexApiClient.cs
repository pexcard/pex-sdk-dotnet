using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
        private readonly string PexCorrelationIdHeaderName = "X-CORRELATION-ID";

        private readonly HttpClient _httpClient;

        public PexApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public Uri BaseUri => _httpClient.BaseAddress;

        public async Task<bool> Ping(CancellationToken token = default(CancellationToken))
        {
            const string url = "v4/ping";
            var response = await _httpClient.GetAsync(url, token);
            return response.IsSuccessStatusCode;
        }

        public async Task<RenewTokenResponseModel> RenewExternalToken(string externalToken,
            CancellationToken token = default(CancellationToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.PostAsync("V4/Token/Renew", null, token);
            var result = await HandleHttpResponseMessage<RenewTokenResponseModel>(response);

            return result;
        }

        public async Task<string> ExchangeJwtForApiToken(string jwt, ExchangeTokenRequestModel exchangeTokenRequest,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Bearer, jwt);

            var content = new StringContent(JsonConvert.SerializeObject(exchangeTokenRequest), Encoding.UTF8,
                "application/json");

            var response =
                await _httpClient.PostAsync("Internal/V4/Account/Token/Exchange", content, cancellationToken);
            var result = await HandleHttpResponseMessage<string>(response);

            return result;
        }

        public async Task DeleteExternalToken(string externalToken, CancellationToken token = default(CancellationToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);
            var response = await _httpClient.DeleteAsync("V4/Token", token);

            await HandleHttpResponseMessage(response);
        }


        public async Task<decimal> GetPexAccountBalance(string externalToken, CancellationToken token = default(CancellationToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var builder = new UriBuilder(new Uri(_httpClient.BaseAddress, "V4/Business/Balance"));

            var response = await _httpClient.GetAsync(builder.Uri, token);
            var result = await HandleHttpResponseMessage<BusinessBalanceModel>(response);

            return result?.BusinessAccountBalance ?? 0;
        }

        public async Task<int> GetAllCardholderTransactionsCount(
            string externalToken,
            DateTime startDate,
            DateTime endDate,
            bool includePendings = false,
            bool includeDeclines = false,
            CancellationToken token = default(CancellationToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var builder = new UriBuilder(new Uri(_httpClient.BaseAddress, "V4/Details/AllCardholderTransactionCount"));

            var query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("IncludePendings", includePendings.ToString());
            query.Add("IncludeDeclines", includeDeclines.ToString());
            query.Add("StartDate", startDate.ToDateTimeString());
            query.Add("EndDate", endDate.ToDateTimeString());
            builder.Query = query.ToString();

            var response = await _httpClient.GetAsync(builder.Uri, token);
            var result = await HandleHttpResponseMessage<int>(response);

            return result;
        }

        public async Task<CardholderTransactions> GetAllCardholderTransactions(
            string externalToken,
            DateTime startDate,
            DateTime endDate,
            bool includePendings = false,
            bool includeDeclines = false,
            CancellationToken token = default(CancellationToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var builder = new UriBuilder(new Uri(_httpClient.BaseAddress, "V4/Details/AllCardholderTransactions"));

            var query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("IncludePendings", includePendings.ToString());
            query.Add("IncludeDeclines", includeDeclines.ToString());
            query.Add("StartDate", startDate.ToDateTimeString());
            query.Add("EndDate", endDate.ToDateTimeString());
            builder.Query = query.ToString();

            var response = await _httpClient.GetAsync(builder.Uri, token);
            var result = await HandleHttpResponseMessage<TransactionListModel>(response);

            return new CardholderTransactions(result?.TransactionList ?? new List<TransactionModel>());
        }

        public async Task<BusinessAccountTransactions> GetBusinessAccountTransactions(
            string externalToken,
            DateTime startDate,
            DateTime endDate,
            bool includePendings = false,
            bool includeDeclines = false,
            CancellationToken token = default(CancellationToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var builder = new UriBuilder(new Uri(_httpClient.BaseAddress, "V4/Details/TransactionDetails"));

            var query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("IncludePendings", includePendings.ToString());
            query.Add("IncludeDeclines", includeDeclines.ToString());
            query.Add("StartDate", startDate.ToDateTimeString());
            query.Add("EndDate", endDate.ToDateTimeString());
            builder.Query = query.ToString();

            var response = await _httpClient.GetAsync(builder.Uri, token);
            var result = await HandleHttpResponseMessage<TransactionListModel>(response);

            return new BusinessAccountTransactions(result?.TransactionList ?? new List<TransactionModel>());
        }

        public async Task<List<AttachmentLinkModel>> GetTransactionAttachments(string externalToken, long transactionId,
            CancellationToken token = default(CancellationToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.GetAsync($"V4/Transactions/{transactionId}/Attachments", token);
            var result = await HandleHttpResponseMessage<AttachmentsModel>(response, true);

            return result?.Attachments;
        }

        public async Task<AttachmentModel> GetTransactionAttachment(string externalToken, long transactionId,
            string attachmentId, CancellationToken token = default(CancellationToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response =
                await _httpClient.GetAsync($"V4/Transactions/{transactionId}/Attachment/{attachmentId}", token);
            var result = await HandleHttpResponseMessage<AttachmentModel>(response, true);

            return result;
        }

        public async Task AddTransactionNote(string externalToken, TransactionModel transaction,
            string noteText, CancellationToken token = default(CancellationToken))
        {
            var noteRequest = new NoteRequestModel
            {
                NoteText = noteText,
                Pending = transaction.IsPending,
                TransactionId = transaction.TransactionId
            };

            const string url = "v4/note";
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);
            var content = new StringContent(JsonConvert.SerializeObject(noteRequest), Encoding.UTF8,
                "application/json");
            var response = await _httpClient.PostAsync(url, content, token);

            await HandleHttpResponseMessage(response);
        }


        public async Task<bool> IsTagsEnabled(string externalToken,
            CancellationToken token = default(CancellationToken))
        {
            var response = await GetTagsResponse(externalToken, token);

            if (response.StatusCode == HttpStatusCode.Forbidden) return false;

            await HandleHttpResponseMessage(response);
            return true;
        }

        public async Task<bool> IsTagsAvailable(string externalToken, CustomFieldType fieldType,
            CancellationToken token = default(CancellationToken))
        {
            var response = await GetTagsResponse(externalToken, token);

            if (response.StatusCode == HttpStatusCode.Forbidden) return false;

            var tags = await HandleHttpResponseMessage<List<TagDetailsModel>>(response);
            var result = tags.Any(x => x.Type == fieldType);
            return result;
        }

        public async Task<GetAdminProfileModel> GetMyAdminProfile(string externalToken, CancellationToken token = default(CancellationToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.GetAsync("V4/Business/MyProfile", token);
            var result = await HandleHttpResponseMessage<GetAdminProfileModel>(response);

            return result;
        }

        /// <summary>
        /// Return all accounts associated with your business.
        /// </summary>
        public async Task<BusinessDetailsModel> GetBusinessDetails(string externalToken,
            CancellationToken token = default(CancellationToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.GetAsync("V4/Details/AccountDetails", token);
            var result = await HandleHttpResponseMessage<BusinessDetailsModel>(response);

            return result;
        }

        public async Task<BusinessSettingsModel> GetBusinessSettings(string externalToken, CancellationToken token = default(CancellationToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.GetAsync("V4/Business/Settings", token);
            var result = await HandleHttpResponseMessage<BusinessSettingsModel>(response);

            return result;
        }

        /// <summary>
        /// Creates a card funding transaction. This transfers money from the business to the card making funds immediately available to spend.
        /// Attaches a note to the latest funding transaction matching the requested amount.
        /// </summary>
        public async Task<FundResponseModel> FundCard(string externalToken, int cardholderAccountId, decimal amount,
           string note = "", CancellationToken token = default(CancellationToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var requestContent = JsonConvert.SerializeObject(
                new FundRequestModel
                {
                    Amount = amount,
                    NoteText = note
                });
            var request = new StringContent(requestContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"V4/Card/Fund/{cardholderAccountId}", request, token);
            var result = await HandleHttpResponseMessage<FundResponseModel>(response);

            return result;
        }

        /// <summary>
        /// Fund a specified card accountID to zero ($0).
        /// </summary>
        public async Task<FundResponseModel> ZeroCard(
            string externalToken,
            int cardholderAccountId,
            CancellationToken token = default)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.PostAsync($"V4/Card/Zero/{cardholderAccountId}", null, token);
            var result = await HandleHttpResponseMessage<FundResponseModel>(response);

            return result;
        }

        public async Task<CardholderTransactions> GetCardholderTransactions(
            string externalToken,
            int cardholderAccountId,
            DateTime startDate,
            DateTime endDate,
            bool includePending = false,
            bool includeDeclines = false,
            CancellationToken token = default(CancellationToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var builder = new UriBuilder(new Uri(_httpClient.BaseAddress, $"V4/Details/TransactionDetails/{cardholderAccountId}"));

            var query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("IncludePendings", includePending.ToString());
            query.Add("IncludeDeclines", includeDeclines.ToString());
            query.Add("StartDate", startDate.ToEst().ToDateTimeString());
            query.Add("EndDate", endDate.ToEst().ToDateTimeString());
            builder.Query = query.ToString();

            var response = await _httpClient.GetAsync(builder.Uri, token);
            var result = await HandleHttpResponseMessage<TransactionListModel>(response);

            return new CardholderTransactions(result.TransactionList ?? new List<TransactionModel>());
        }


        public async Task<CardholderDetailsModel> GetCardholderDetails(string externalToken,
            int cardholderAccountId, CancellationToken token = default(CancellationToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.GetAsync($"V4/Details/AccountDetails/{cardholderAccountId}", token);
            var result = await HandleHttpResponseMessage<CardholderDetailsModel>(response);

            return result;
        }

        public async Task<CardholderProfileModel> GetCardholderProfile(string externalToken,
            int cardholderAccountId, CancellationToken token = default(CancellationToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.GetAsync($"V4/Card/Profile/{cardholderAccountId}", token);
            var result = await HandleHttpResponseMessage<CardholderProfileModel>(response);

            return result;
        }

        public async Task<List<TagDetailsModel>> GetTags(string externalToken, CancellationToken token = default(CancellationToken))
        {
            var response = await GetTagsResponse(externalToken, token);
            var result = await HandleHttpResponseMessage<List<TagDetailsModel>>(response);

            return result;
        }

        public async Task<TagDetailsModel> GetTag(string externalToken, string tagId,
            CancellationToken token = default(CancellationToken))
        {
            var result = await GetTag<TagDetailsModel>(externalToken, tagId, token);
            return result;
        }

        public async Task<TagDropdownDetailsModel> GetDropdownTag(string externalToken, string tagId,
            CancellationToken token = default(CancellationToken))
        {
            var result = await GetTag<TagDropdownDetailsModel>(externalToken, tagId, token);
            return result;
        }

        public async Task<TagDropdownDetailsModel> CreateDropdownTag(string externalToken, TagDropdownDataModel tag,
            CancellationToken token = default(CancellationToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var requestContent = JsonConvert.SerializeObject(tag);
            var request = new StringContent(requestContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("V4/Business/Configuration/Tag/Dropdown", request, token);
            var result = await HandleHttpResponseMessage<TagDropdownDetailsModel>(response);

            return result;
        }

        public async Task<TagDropdownDetailsModel> UpdateDropdownTag(string externalToken, string tagId, TagDropdownModel tag,
            CancellationToken token = default(CancellationToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var requestContent = JsonConvert.SerializeObject(tag);
            var request = new StringContent(requestContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"V4/Business/Configuration/Tag/Dropdown/{tagId}", request, token);
            var result = await HandleHttpResponseMessage<TagDropdownDetailsModel>(response);

            return result;
        }

        public async Task<TagDropdownDetailsModel> DeleteDropdownTag(string externalToken, string tagId,
            CancellationToken token = default(CancellationToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.DeleteAsync($"V4/Business/Configuration/Tag/Dropdown/{tagId}", token);
            var result = await HandleHttpResponseMessage<TagDropdownDetailsModel>(response);

            return result;
        }

        public async Task<TokenResponseModel> GetTokens(string externalToken, CancellationToken token = default(CancellationToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.GetAsync("V4/Token", token);
            var result = await HandleHttpResponseMessage<TokenResponseModel>(response);
            return result;
        }

        public async Task<TokenDataModel> GetToken(string externalToken, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tokens = await GetTokens(externalToken, cancellationToken);

            var token = tokens.Tokens.FirstOrDefault(pt => pt.Token == externalToken);
            return token;
        }

        public async Task<int> CreateCardOrder(string externalToken, CardOrderModel cardOrder, CancellationToken token = default(CancellationToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var requestContent = JsonConvert.SerializeObject(cardOrder);
            var request = new StringContent(requestContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("V4/Card/CreateAsync", request, token);
            var result = await HandleHttpResponseMessage<CardOrderIdModel>(response);

            return result.CardOrderId;
        }

        public async Task<PartnerModel> GetPartner(string externalToken, CancellationToken token = default(CancellationToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.GetAsync($"v4/Partner", token);
            var result = await HandleHttpResponseMessage<PartnerModel>(response);

            return result;
        }

        public async Task<CardholderGroupsResponseModel> GetCardholderGroups(string externalToken, CancellationToken token = default)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.GetAsync($"v4/Group", token);
            var result = await HandleHttpResponseMessage<CardholderGroupsResponseModel>(response);

            return result;
        }

        public async Task<CardholderGroupResponseModel> GetCardholderGroup(string externalToken, int groupId, CancellationToken token = default)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.GetAsync($"v4/Group/{groupId}", token);
            var result = await HandleHttpResponseMessage<CardholderGroupResponseModel>(response);

            return result;
        }

        public async Task<CardholderGroupResponseModel> CreateCardholderGroup(string externalToken, string groupName, CancellationToken token = default)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var requestContent = JsonConvert.SerializeObject(new UpsertCardholderGroupModel { Name = groupName });
            var request = new StringContent(requestContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("V4/Group", request, token);
            var result = await HandleHttpResponseMessage<CardholderGroupResponseModel>(response);

            return result;
        }

        public async Task<CardholderGroupResponseModel> UpdateCardholderGroupName(string externalToken, int groupId, string groupName, CancellationToken token = default)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var requestContent = JsonConvert.SerializeObject(new UpsertCardholderGroupModel { Name = groupName });
            var request = new StringContent(requestContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"V4/Group/{groupId}", request, token);
            var result = await HandleHttpResponseMessage<CardholderGroupResponseModel>(response);

            return result;
        }

        public async Task DeleteCardholderGroup(string externalToken, int groupId, CancellationToken token = default)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.DeleteAsync($"V4/Group/{groupId}", token);
            await HandleHttpResponseMessage<CardholderGroupModel>(response);
        }

        public async Task<TagsModel> GetTransactionTags(string externalToken, long transactionId, CancellationToken token = default)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.GetAsync($"V4/Transactions/{transactionId}/Tags", token);
            return await HandleHttpResponseMessage<TagsModel>(response);
        }

        public async Task AddTransactionTags(string externalToken, long transactionId, UpsertTransactionTagsModel transactionTags, CancellationToken token = default)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var requestContent = JsonConvert.SerializeObject(transactionTags);
            var request = new StringContent(requestContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"V4/Transactions/{transactionId}/Tags", request, token);
            await HandleHttpResponseMessage(response);
        }

        public async Task UpdateTransactionTags(string externalToken, long transactionId, UpsertTransactionTagsModel transactionTags, CancellationToken token = default)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var requestContent = JsonConvert.SerializeObject(transactionTags);
            var request = new StringContent(requestContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"V4/Transactions/{transactionId}/Tags", request, token);
            await HandleHttpResponseMessage(response);
        }

        #region Private methods

        private async Task<HttpResponseMessage> GetTagsResponse(string externalToken, CancellationToken token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var result = await _httpClient.GetAsync("V4/Business/Configuration/Tags", token);
            return result;
        }

        private async Task<T> GetTag<T>(string externalToken, string tagId, CancellationToken token) where T : TagDataModel
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TokenType.Token, externalToken);

            var response = await _httpClient.GetAsync($"V4/Business/Configuration/Tag/{tagId}", token);
            var result = await HandleHttpResponseMessage<T>(response);

            return result;
        }

        private async Task<T> HandleHttpResponseMessage<T>(HttpResponseMessage response, bool notFoundAsDefault = false)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            try
            {
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NotFound && notFoundAsDefault)
                    {
                        return default(T);
                    }

                    var errorModel = JsonConvert.DeserializeObject<ErrorMessageModel>(responseContent);
                    var correlationId = GetPexCorrelationId(response);

                    throw new PexApiClientException(response.StatusCode, errorModel.Message, correlationId);
                }
                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            catch (Exception ex)
            {
                var correlationId = GetPexCorrelationId(response);

                throw new PexApiClientException(response.StatusCode, $"Error parsing response: {ex.Message}\nContent: {responseContent}", ex, correlationId);
            }
        }

        private async Task HandleHttpResponseMessage(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                try
                {
                    var errorModel = JsonConvert.DeserializeObject<ErrorMessageModel>(responseContent);
                    var correlationId = GetPexCorrelationId(response);

                    throw new PexApiClientException(response.StatusCode, errorModel.Message, correlationId);
                }
                catch (Exception ex)
                {
                    var correlationId = GetPexCorrelationId(response);

                    throw new PexApiClientException(response.StatusCode, $"Error parsing response: {ex.Message}\nContent: {responseContent}", ex, correlationId);
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
