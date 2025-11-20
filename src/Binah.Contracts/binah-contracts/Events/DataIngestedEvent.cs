using System;
using System.Collections.Generic;

namespace Binah.Contracts.Events;

/// <summary>
/// Event published when data is ingested during pipeline execution
/// Published for each batch of entities created
/// </summary>
public class DataIngestedEvent : OntologyEvent
{
    public override string EventType => "pipeline.data.ingested";

    /// <summary>
    /// ID of the pipeline that ingested the data
    /// </summary>
    public string PipelineId { get; set; } = string.Empty;

    /// <summary>
    /// Name of the pipeline
    /// </summary>
    public string PipelineName { get; set; } = string.Empty;

    /// <summary>
    /// Execution ID for tracking this specific run
    /// </summary>
    public string ExecutionId { get; set; } = string.Empty;

    /// <summary>
    /// Entity type being ingested (Property, Transaction, etc.)
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Number of entities in this batch
    /// </summary>
    public int BatchSize { get; set; }

    /// <summary>
    /// List of entity IDs in this batch
    /// </summary>
    public List<string> EntityIds { get; set; } = new();

    /// <summary>
    /// Source node ID (for node-graph pipelines)
    /// </summary>
    public string? SourceNodeId { get; set; }

    /// <summary>
    /// Destination node ID (for node-graph pipelines)
    /// </summary>
    public string? DestinationNodeId { get; set; }

    /// <summary>
    /// When this batch was ingested
    /// </summary>
    public DateTime IngestedAt { get; set; } = DateTime.UtcNow;
}
