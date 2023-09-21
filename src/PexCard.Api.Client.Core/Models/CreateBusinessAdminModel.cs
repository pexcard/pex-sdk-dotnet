namespace PexCard.Api.Client.Core.Models
{
    public class CreateBusinessAdminModel : UpdateBusinessAdminModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}