using System;
using System.Collections.Generic;

namespace Binah.Contracts.Events;

/// <summary>
/// Event published when a pipeline execution completes
/// </summary>
public class PipelineCompletionEvent : OntologyEvent
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
    /// Execution ID for this specific run
    /// </summary>
    public string ExecutionId { get; set; } = string.Empty;

    /// <summary>
    /// List of entity IDs created during this pipeline execution
    /// </summary>
    public List<string> EntitiesCreated { get; set; } = new();

    /// <summary>
    /// List of entity IDs updated during this pipeline execution
    /// </summary>
    public List<string> EntitiesUpdated { get; set; } = new();

    /// <summary>
    /// Number of entities successfully processed
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of entities that failed processing
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// When the pipeline execution completed
    /// </summary>
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Duration of the pipeline execution in milliseconds
    /// </summary>
    public long DurationMs { get; set; }

    /// <summary>
    /// Status of the pipeline execution (success, partial_success, failed)
    /// </summary>
    public string Status { get; set; } = "success";

    /// <summary>
    /// Error message if pipeline failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}
