using System;

namespace Binah.Contracts.Events;

/// <summary>
/// Event published when a pipeline execution starts
/// </summary>
public class PipelineStartedEvent : OntologyEvent
{
    public override string EventType => "pipeline.execution.started";

    /// <summary>
    /// ID of the pipeline being executed
    /// </summary>
    public string PipelineId { get; set; } = string.Empty;

    /// <summary>
    /// Name of the pipeline
    /// </summary>
    public string PipelineName { get; set; } = string.Empty;

    /// <summary>
    /// Source type (MLS, Salesforce, CSV, etc.)
    /// </summary>
    public string SourceType { get; set; } = string.Empty;

    /// <summary>
    /// Execution ID for tracking this specific run
    /// </summary>
    public string ExecutionId { get; set; } = string.Empty;

    /// <summary>
    /// When the pipeline started
    /// </summary>
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Pipeline status at start
    /// </summary>
    public string Status { get; set; } = "running";
}
