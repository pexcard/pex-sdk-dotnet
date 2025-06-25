namespace PexCard.Api.Client.Core.Models
{
    public class CreateTokenRequestModel
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string UserAgentString { get; set; }
    }
}