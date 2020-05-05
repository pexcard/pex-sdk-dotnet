using System;

namespace PexCard.Api.Client.Core.Models
{
    public class TokenDataModel
    {
        /// <summary>Username associated with the token</summary>
        public string Username { get; set; }
        /// <summary>AppId associated with the token</summary>
        public string AppId { get; set; }
        /// <summary>3scale access token</summary>
        public string Token { get; set; }
        /// <summary>Time token will expire</summary>
        public DateTime TokenExpiration { get; set; }
    }
}
