using System;
using System.Threading;
using System.Threading.Tasks;

namespace PexCard.Api.Client.Core
{
    public interface IPexAuthClient
    {
        Task<Uri> GetPartnerAuthUriAsync(string appId, string appSecret, Uri serverCallbackUri, Uri browserClosingUri, CancellationToken cancelToken = default);

        Task RevokePartnerAuthTokenAsync(string appId, string appSecret, string token, CancellationToken cancelToken = default);

        Task<string> GetTokenAsync(string appId, string appSecret, string username, string password, string userAgentString, CancellationToken cancelToken = default);
    }
}
