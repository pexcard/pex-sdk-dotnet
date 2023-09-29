using System;

namespace PexCard.Api.Client.Core.Models
{
    public class BusinessProfileModel
    {
        public int BusinessAcctId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneExtension { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public DateTime DateOfBirth { get; set; }

        public AddressContactModel ProfileAddress { get; set; }
    }
}