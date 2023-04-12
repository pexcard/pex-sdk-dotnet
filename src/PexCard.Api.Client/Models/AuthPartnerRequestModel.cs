using System;
using System.Runtime.Serialization;

namespace PexCard.Api.Client.Models
{
    [DataContract]
    internal class AuthPartnerRequestModel
    {
        public AuthPartnerRequestModel(string appId, string appSecret, Uri serverCallbackUrl, Uri browserClosingUrl)
        {
            AppId = appId;
            AppSecret = appSecret;
            ServerCallbackUrl = serverCallbackUrl;
            BrowserClosingUrl = browserClosingUrl;
        }

        [DataMember(Order = 1)]
        public string AppId { get; set; }

        [DataMember(Order = 2)]
        public string AppSecret { get; set; }

        [DataMember(Order = 3)]
        public Uri ServerCallbackUrl { get; set; }

        [DataMember(Order = 4)]
        public Uri BrowserClosingUrl { get; set; }
    }
}
