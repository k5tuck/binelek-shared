using System;
using System.Collections.Generic;

namespace Binah.Contracts.Events;

/// <summary>
/// Event published when enrichment is requested for an entity
/// </summary>
public class EnrichmentRequestEvent : OntologyEvent
{
    public override string EventType => "enrichment.requested";

    /// <summary>
    /// ID of the entity to be enriched
    /// </summary>
    public string EntityId { get; set; } = string.Empty;

    /// <summary>
    /// Type of enrichment requested (geocoding, property_details, credit_score, etc.)
    /// </summary>
    public string EnrichmentType { get; set; } = string.Empty;

    /// <summary>
    /// Parameters for enrichment request
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = new();

    /// <summary>
    /// Priority level (1-5, where 1 is highest)
    /// </summary>
    public int Priority { get; set; } = 3;

    /// <summary>
    /// Source service that requested enrichment
    /// </summary>
    public string? RequestSource { get; set; }
}
