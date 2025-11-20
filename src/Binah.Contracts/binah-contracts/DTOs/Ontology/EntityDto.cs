using System;
using System.Collections.Generic;

namespace Binah.Contracts.DTOs.Ontology;

/// <summary>
/// Data transfer object for Entity
/// </summary>
public class EntityDto
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Entity type (e.g., Project, Contractor)
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Entity properties as key-value pairs
    /// </summary>
    public Dictionary<string, object> Properties { get; set; } = new();

    /// <summary>
    /// Version number for optimistic concurrency
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// User who created the entity
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// User who last updated the entity
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Tenant identifier for multi-tenancy
    /// </summary>
    public string? TenantId { get; set; }
}
