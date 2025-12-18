namespace PexCard.Api.Client.Core.Enums
{
    /// <summary>
    /// Status triggers allowed for vendor status update operations.
    /// This is a subset of VendorStatusTrigger that excludes triggers
    /// that require dedicated endpoints (Rejected) or are only used
    /// during vendor creation (New).
    /// </summary>
    public enum VendorStatusUpdateTrigger
    {
        Active = 30,
        Inactive = 35,
        Offboarded = 45
    }
}
