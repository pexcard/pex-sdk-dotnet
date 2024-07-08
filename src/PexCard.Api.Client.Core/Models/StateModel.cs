using PexCard.Api.Client.Core.Enums;

namespace PexCard.Api.Client.Core.Models;

public class StateModel
{
    public MetadataStateType State { get; set; }
    public string Reason { get; set; }
    public long? ApprovalId { get; set; }
    public string MetadataId { get; set; }
    public long TransactionRelationshipId { get; set; }
    public MetadataType MetadataType { get; set; }
    public int BusinessId { get; set; }
}