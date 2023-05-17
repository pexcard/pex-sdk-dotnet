namespace PexCard.Api.Client.Core.Models
{
    public enum CardStatus
    {
        /// <summary>Invalid card status</summary>
        Invalid = -1,
        /// <summary>Card is active</summary>
        Active = 0,
        /// <summary>Card is inactive</summary>
        Inactive = 1,
        /// <summary>Card is blocked</summary>
        Blocked = 2,
        /// <summary>Card is closed</summary>
        Closed = 5,

        Removed = 11
    }
}
