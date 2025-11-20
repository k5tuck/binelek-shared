using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Binah.Contracts.DTOs.Ontology;

/// <summary>
/// Request to update an existing entity
/// </summary>
public class UpdateEntityRequest
{
    /// <summary>
    /// Properties to update
    /// </summary>
    [Required(ErrorMessage = "Properties are required")]
    [MinLength(1, ErrorMessage = "At least one property is required")]
    public Dictionary<string, object> Properties { get; set; } = new();

    /// <summary>
    /// User updating the entity
    /// </summary>
    [StringLength(50, ErrorMessage = "UpdatedBy must not exceed 50 characters")]
    public string? UpdatedBy { get; set; }
}
