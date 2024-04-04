using System;

namespace PexCard.Api.Client.Core.Models
{
    public class TokenDataModel
    {
        /// <summary>AppId associated with the token</summary>
        /// 
        public string AppId { get; set; }
        
        /// <summary>BusinessAccountId associated with the token</summary>
        public int BusinessAccountId { get; set; }

        /// <summary>UserType associated with the token</summary>
        public UserType UserType { get; set; }

        /// <summary>Username associated with the token</summary>
        public string Username { get; set; }

        /// <summary>3scale access token</summary>
        public string Token { get; set; }

        /// <summary>Time token will expire</summary>
        public DateTime TokenExpiration { get; set; }
    }
}
