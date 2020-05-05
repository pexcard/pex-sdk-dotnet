using System;

namespace PexCard.Api.Client.Core.Models
{
    public class RenewTokenResponseModel
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public string AppId { get; set; }
        public DateTime TokenExpiration { get; set; }
    }
}
