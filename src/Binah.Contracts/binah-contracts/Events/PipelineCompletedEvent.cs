using System;
using System.Collections.Generic;

namespace Binah.Contracts.Events;

/// <summary>
/// Event published when a pipeline execution completes successfully
/// </summary>
public class PipelineCompletedEvent : OntologyEvent
{
    public override string EventType => "pipeline.execution.completed";

    /// <summary>
    /// ID of the pipeline that completed
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
    /// List of entity IDs created during pipeline execution
    /// </summary>
    public List<string> EntitiesCreated { get; set; } = new();

    /// <summary>
    /// Number of rows successfully processed
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of rows that failed processing
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Total rows processed (success + failure)
    /// </summary>
    public int TotalProcessed { get; set; }

    /// <summary>
    /// When the pipeline completed
    /// </summary>
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Pipeline execution duration
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Destination entity type (Property, Transaction, etc.)
    /// </summary>
    public string? EntityType { get; set; }
}
