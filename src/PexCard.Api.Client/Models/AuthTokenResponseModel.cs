using System.Runtime.Serialization;

namespace PexCard.Api.Client.Models
{
    [DataContract]
    internal class AuthTokenResponseModel
    {
        public AuthTokenResponseModel()
        {
        }

        public AuthTokenResponseModel(string token)
        {
            Token = token;
        }

        [DataMember(Order = 1)]
        public string Token { get; set; }
    }
}