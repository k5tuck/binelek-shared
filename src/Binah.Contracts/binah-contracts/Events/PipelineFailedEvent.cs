using System;

namespace Binah.Contracts.Events;

/// <summary>
/// Event published when a pipeline execution fails
/// </summary>
public class PipelineFailedEvent : OntologyEvent
{
    public override string EventType => "pipeline.execution.failed";

    /// <summary>
    /// ID of the pipeline that failed
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
    /// Error message describing the failure
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Stack trace for debugging (sanitized for security)
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// Number of rows successfully processed before failure
    /// </summary>
    public int ProcessedCount { get; set; }

    /// <summary>
    /// Number of rows that succeeded before failure
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// When the pipeline failed
    /// </summary>
    public DateTime FailedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Stage at which the pipeline failed (source, transformation, destination)
    /// </summary>
    public string? FailureStage { get; set; }

    /// <summary>
    /// Node ID where failure occurred (for node-graph pipelines)
    /// </summary>
    public string? FailedNodeId { get; set; }
}
