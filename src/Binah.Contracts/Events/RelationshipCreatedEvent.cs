namespace Binah.Contracts.Events;

/// <summary>
/// Event published when a relationship is created
/// </summary>
public class RelationshipCreatedEvent : OntologyEvent
{
    public override string EventType => "relationship.created";

    /// <summary>
    /// ID of the created relationship
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
