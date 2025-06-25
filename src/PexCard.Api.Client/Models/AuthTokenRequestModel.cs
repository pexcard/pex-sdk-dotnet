namespace PexCard.Api.Client.Models
{
    internal class AuthTokenRequestModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string UserAgentString { get; set; }
    }
}