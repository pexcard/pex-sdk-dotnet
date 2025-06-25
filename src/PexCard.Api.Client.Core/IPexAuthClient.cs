using System;
using System.Threading;
using System.Threading.Tasks;
using PexCard.Api.Client.Core.Models;

namespace PexCard.Api.Client.Core
{
    public interface IPexAuthClient
    {
        Task<Uri> GetPartnerAuthUriAsync(string appId, string appSecret, Uri serverCallbackUri, Uri browserClosingUri, CancellationToken cancelToken = default);

        Task RevokePartnerAuthTokenAsync(string appId, string appSecret, string token, CancellationToken cancelToken = default);

        Task<CreateTokenResponseModel> CreateToken(CreateTokenRequestModel request, CancellationToken cancelToken = default);
    }
}
