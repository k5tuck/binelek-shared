using System.ComponentModel.DataAnnotations;
using YamlDotNet.Serialization;

namespace Binah.Contracts.Models;

/// <summary>
/// Defines the structure of an entity in the domain ontology
/// </summary>
public class EntityDefinition
{
    [Required]
    [YamlMember(Alias = "name")]
    public string Name { get; set; } = null!;

    [YamlMember(Alias = "description")]
    public string? Description { get; set; }

    [YamlMember(Alias = "icon")]
    public string? Icon { get; set; }

    [YamlMember(Alias = "color")]
    public string? Color { get; set; }

    [YamlMember(Alias = "extends")]
    public string? Extends { get; set; }

    [Required]
    [YamlMember(Alias = "attributes")]
    public List<AttributeDefinition> Attributes { get; set; } = new();

    [YamlMember(Alias = "validation")]
    public Dictionary<string, object>? Validation { get; set; }
}

/// <summary>
/// Defines an attribute (property) of an entity
/// </summary>
public class AttributeDefinition
{
    [Required]
    [YamlMember(Alias = "name")]
    public string Name { get; set; } = null!;

    [Required]
    [YamlMember(Alias = "type")]
    public string Type { get; set; } = null!;

    [YamlMember(Alias = "description")]
    public string? Description { get; set; }

    [YamlMember(Alias = "required")]
    public bool Required { get; set; }

    [YamlMember(Alias = "indexed")]
    public bool Indexed { get; set; }

    [YamlMember(Alias = "unique")]
    public bool Unique { get; set; }

    [YamlMember(Alias = "default")]
    public object? Default { get; set; }

    [YamlMember(Alias = "min")]
    public double? Min { get; set; }

    [YamlMember(Alias = "max")]
    public double? Max { get; set; }

    [YamlMember(Alias = "min_length")]
    public int? MinLength { get; set; }

    [YamlMember(Alias = "max_length")]
    public int? MaxLength { get; set; }

    [YamlMember(Alias = "pattern")]
    public string? Pattern { get; set; }

    [YamlMember(Alias = "values")]
    public List<string>? Values { get; set; }  // For enum types

    [YamlMember(Alias = "unit")]
    public string? Unit { get; set; }

    [YamlMember(Alias = "format")]
    public string? Format { get; set; }
}

/// <summary>
/// Defines a relationship between two entities in the domain ontology
/// </summary>
public class RelationshipDefinition
{
    [Required]
    [YamlMember(Alias = "name")]
    public string Name { get; set; } = null!;

    [Required]
    [YamlMember(Alias = "from")]
    public string From { get; set; } = null!;

    [Required]
    [YamlMember(Alias = "to")]
    public string To { get; set; } = null!;

    [YamlMember(Alias = "type")]
    public string Type { get; set; } = "one-to-many";  // one-to-one, one-to-many, many-to-many

    [YamlMember(Alias = "description")]
    public string? Description { get; set; }

    [YamlMember(Alias = "properties")]
    public Dictionary<string, object>? Properties { get; set; }
}
