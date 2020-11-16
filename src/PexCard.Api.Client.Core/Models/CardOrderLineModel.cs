using System;

namespace PexCard.Api.Client.Core.Models
{
    public class CardOrderLineModel
    {
        /// <summary>Phone number</summary>
        public string Phone{set;get;}

        public string ShippingPhone {set; get;}

        /// <summary>Method for shipping the card</summary>
        public ShippingMethod ShippingMethod { get; set; }

        /// <summary>Cardholder date of birth</summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>Cardholder email address</summary>
        public string Email { get; set; }

        /// <summary> Cardholder group id </summary>
        public int? GroupId { get; set; }

        /// <summary> Cardhodler ruleset id </summary>
        public int? RulesetId { get; set; }

        /// <summary>User defined Id which can be assigned to Card holder profile (alphanumeric up to 50 characters)</summary>
        public string CustomId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool NormalizeProfileAddress { get; set; }

        public AddressModel ProfileAddress { get; set; }

        public bool NormalizeShippingAddress { get; set; }

        public AddressContactModel ShippingAddress { get; set; }
    }

    public enum ShippingMethod
    {
        // free
        FirstClassMail = 0,

        // thirty-five
        Expedited = 1,

        // forty-five
        Rush = 2
    }
}
