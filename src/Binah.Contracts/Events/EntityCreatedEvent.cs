using System.Collections.Generic;

namespace Binah.Contracts.Events;

/// <summary>
/// Event published when an entity is created
/// </summary>
public class EntityCreatedEvent : OntologyEvent
{
    public override string EventType => "entity.created";

    /// <summary>
    /// ID of the created entity
    /// </summary>
    public string EntityId { get; set; } = string.Empty;

    /// <summary>
    /// Type of the created entity
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Entity properties
    /// </summary>
    public Dictionary<string, object> Properties { get; set; } = new();
}
