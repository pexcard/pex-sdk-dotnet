using System;
using System.Runtime.Serialization;

namespace PexCard.Api.Client.Models
{
    [DataContract]
    internal class AuthPartnerResponseModel
    {
        public AuthPartnerResponseModel()
        {
        }

        public AuthPartnerResponseModel(Uri oauthUrl)
        {
            OauthUrl = oauthUrl;
        }

        [DataMember(Order = 1)]
        public Uri OauthUrl { get; set; }
    }
}
