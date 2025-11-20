using System;

namespace Binah.Contracts.DTOs.Ontology;

/// <summary>
/// Data transfer object for Relationship
/// </summary>
public class RelationshipDto
{
    /// <summary>
    /// Unique identifier for the relationship
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Relationship type (e.g., MANAGES, DEPENDS_ON)
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Source entity ID
    /// </summary>
    public string SourceId { get; set; } = string.Empty;

    /// <summary>
    /// Target entity ID
    /// </summary>
    public string TargetId { get; set; } = string.Empty;

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// User who created the relationship
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Tenant identifier for multi-tenancy
    /// </summary>
    public string? TenantId { get; set; }
}
