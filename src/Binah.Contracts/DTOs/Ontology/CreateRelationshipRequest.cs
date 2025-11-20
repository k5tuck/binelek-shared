using System.ComponentModel.DataAnnotations;

namespace Binah.Contracts.DTOs.Ontology;

/// <summary>
/// Request to create a new relationship between entities
/// </summary>
public class CreateRelationshipRequest
{
    /// <summary>
    /// Relationship type (required)
    /// </summary>
    [Required(ErrorMessage = "Relationship type is required")]
    [StringLength(50, ErrorMessage = "Relationship type must not exceed 50 characters")]
    [RegularExpression(@"^[A-Z_]+$",
        ErrorMessage = "Relationship type must be UPPER_CASE (e.g., MANAGES, DEPENDS_ON)")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Source entity ID (required)
    /// </summary>
    [Required(ErrorMessage = "Source ID is required")]
    public string SourceId { get; set; } = string.Empty;

    /// <summary>
    /// Target entity ID (required)
    /// </summary>
    [Required(ErrorMessage = "Target ID is required")]
    public string TargetId { get; set; } = string.Empty;

    /// <summary>
    /// User creating the relationship
    /// </summary>
    [StringLength(50, ErrorMessage = "CreatedBy must not exceed 50 characters")]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Tenant identifier
    /// </summary>
    [StringLength(50, ErrorMessage = "TenantId must not exceed 50 characters")]
    public string? TenantId { get; set; }
}
