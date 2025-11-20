using System;
using System.Collections.Generic;

namespace Binah.Contracts.Events;

/// <summary>
/// Base class for all ontology events
/// </summary>
public abstract class OntologyEvent
{
    /// <summary>
    /// Unique event identifier
    /// </summary>
    public string EventId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Event type
    /// </summary>
    public abstract string EventType { get; }

    /// <summary>
    /// Event timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Event version (for schema evolution)
    /// </summary>
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// User who triggered the event
    /// </summary>
    public string? TriggeredBy { get; set; }

    /// <summary>
    /// Tenant identifier
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Correlation ID for request tracking
    /// </summary>
    public string? CorrelationId { get; set; }
}
