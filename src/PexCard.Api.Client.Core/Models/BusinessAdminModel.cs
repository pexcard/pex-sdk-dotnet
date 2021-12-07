using System;

namespace PexCard.Api.Client.Core.Models
{
    public class BusinessAdminModel
    {
        public int BusinessAdminId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public AddressContactModel ProfileAddress { get; set; }

        public string Phone { get; set; }

        public string AltPhone { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Email { get; set; }

        public AdminPermissionsModel Permissions { get; set; }

        public string Status { get; set; }
    }
}