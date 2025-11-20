using System.Collections.Generic;

namespace Binah.Contracts.Events;

/// <summary>
/// Event published when an entity is updated
/// </summary>
public class EntityUpdatedEvent : OntologyEvent
{
    public override string EventType => "entity.updated";

    /// <summary>
    /// ID of the updated entity
    /// </summary>
    public string EntityId { get; set; } = string.Empty;

    /// <summary>
    /// Type of the updated entity
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Updated properties
    /// </summary>
    public Dictionary<string, object> UpdatedProperties { get; set; } = new();

    /// <summary>
    /// Previous properties (before update)
    /// </summary>
    public Dictionary<string, object>? PreviousProperties { get; set; }
}
