namespace Binah.Contracts.Events;

/// <summary>
/// Event published when an entity is deleted
/// </summary>
public class EntityDeletedEvent : OntologyEvent
{
    public override string EventType => "entity.deleted";

    /// <summary>
    /// ID of the deleted entity
    /// </summary>
    public string EntityId { get; set; } = string.Empty;

    /// <summary>
    /// Type of the deleted entity
    /// </summary>
    public string EntityType { get; set; } = string.Empty;
}
