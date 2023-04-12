using System.Runtime.Serialization;

namespace PexCard.Api.Client.Models
{
    [DataContract]
    internal class AuthRevokeTokenRequestModel
    {
        public AuthRevokeTokenRequestModel(string appId, string appSecret, string token)
        {
            AppId = appId;
            AppSecret = appSecret;
            Token = token;
        }

        [DataMember(Order = 1)]
        public string AppId { get; set; }

        [DataMember(Order = 2)]
        public string AppSecret { get; set; }

        [DataMember(Order = 3)]
        public string Token { get; set; }
    }
}
