namespace Binah.Contracts.Events;

/// <summary>
/// Event published when a relationship is deleted
/// </summary>
public class RelationshipDeletedEvent : OntologyEvent
{
    public override string EventType => "relationship.deleted";

    /// <summary>
    /// ID of the deleted relationship
    /// </summary>
    public string RelationshipId { get; set; } = string.Empty;

    /// <summary>
    /// Type of the relationship
    /// </summary>
    public string RelationshipType { get; set; } = string.Empty;

    /// <summary>
    /// Source entity ID
    /// </summary>
    public string SourceId { get; set; } = string.Empty;

    /// <summary>
    /// Target entity ID
    /// </summary>
    public string TargetId { get; set; } = string.Empty;
}
