using System.Runtime.Serialization;

namespace PexCard.Api.Client.Models
{
    [DataContract]
    internal class AuthTokenRequestModel
    {
        public AuthTokenRequestModel(string username, string password, string userAgentString)
        {
            Username = username;
            Password = password;
            UserAgentString = userAgentString;
        }

        [DataMember(Order = 1)]
        public string Username { get; set; }

        [DataMember(Order = 2)]
        public string Password { get; set; }

        [DataMember(Order = 3)]
        public string UserAgentString { get; set; }
    }
}