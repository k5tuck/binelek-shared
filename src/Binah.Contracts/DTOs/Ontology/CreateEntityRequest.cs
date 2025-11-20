using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Binah.Contracts.DTOs.Ontology;

/// <summary>
/// Request to create a new entity
/// </summary>
public class CreateEntityRequest
{
    /// <summary>
    /// Entity type (required)
    /// </summary>
    [Required(ErrorMessage = "Entity type is required")]
    [StringLength(100, ErrorMessage = "Entity type must not exceed 100 characters")]
    [RegularExpression(@"^[A-Z][a-zA-Z0-9]*$",
        ErrorMessage = "Entity type must be PascalCase (e.g., Project, Contractor)")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Entity properties
    /// </summary>
    [Required(ErrorMessage = "Properties are required")]
    [MinLength(1, ErrorMessage = "At least one property is required")]
    public Dictionary<string, object> Properties { get; set; } = new();

    /// <summary>
    /// User creating the entity
    /// </summary>
    [StringLength(50, ErrorMessage = "CreatedBy must not exceed 50 characters")]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Tenant identifier
    /// </summary>
    [StringLength(50, ErrorMessage = "TenantId must not exceed 50 characters")]
    public string? TenantId { get; set; }
}
