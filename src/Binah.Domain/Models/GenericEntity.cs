using System.Text.Json;
using System.Text.Json.Serialization;

namespace Binah.Core.Domain.Models;

/// <summary>
/// Generic entity that can represent any domain object (Property, Patient, Account, InfrastructureAsset, etc.)
/// The entity type and attributes are determined by the active domain configuration
/// </summary>
public class GenericEntity
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Entity type name (e.g., "Property", "Patient", "InfrastructureAsset")
    /// Maps to entity definitions in domain ontology
    /// </summary>
    [JsonPropertyName("entity_type")]
    public string EntityType { get; set; } = null!;

    /// <summary>
    /// Tenant ID for multi-tenancy
    /// </summary>
    [JsonPropertyName("tenant_id")]
    public string TenantId { get; set; } = null!;

    /// <summary>
    /// Domain ID (e.g., "real-estate", "smart-cities", "healthcare")
    /// Determines which ontology schema to use
    /// </summary>
    [JsonPropertyName("domain_id")]
    public string DomainId { get; set; } = null!;

    /// <summary>
    /// Dynamic attributes specific to the entity type
    /// Structure is defined by the domain's ontology configuration
    /// </summary>
    [JsonPropertyName("attributes")]
    public Dictionary<string, object?> Attributes { get; set; } = new();

    /// <summary>
    /// Entity metadata (created/updated timestamps, version, etc.)
    /// </summary>
    [JsonPropertyName("metadata")]
    public EntityMetadata Metadata { get; set; } = new();

    /// <summary>
    /// Labels for graph database (Neo4j)
    /// Typically includes ["Entity", {EntityType}, {Domain}]
    /// </summary>
    [JsonPropertyName("labels")]
    public List<string> Labels { get; set; } = new();

    /// <summary>
    /// Get an attribute value with type safety
    /// </summary>
    public T? GetAttribute<T>(string attributeName)
    {
        if (!Attributes.TryGetValue(attributeName, out var value) || value == null)
        {
            return default;
        }

        try
        {
            if (value is JsonElement jsonElement)
            {
                return JsonSerializer.Deserialize<T>(jsonElement.GetRawText());
            }

            if (value is T typedValue)
            {
                return typedValue;
            }

            // Try conversion
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Set an attribute value
    /// </summary>
    public void SetAttribute(string attributeName, object? value)
    {
        Attributes[attributeName] = value;
    }

    /// <summary>
    /// Check if attribute exists
    /// </summary>
    public bool HasAttribute(string attributeName)
    {
        return Attributes.ContainsKey(attributeName);
    }

    /// <summary>
    /// Get attribute as string (convenience method)
    /// </summary>
    public string? GetString(string attributeName) => GetAttribute<string>(attributeName);

    /// <summary>
    /// Get attribute as int (convenience method)
    /// </summary>
    public int? GetInt(string attributeName) => GetAttribute<int>(attributeName);

    /// <summary>
    /// Get attribute as decimal (convenience method)
    /// </summary>
    public decimal? GetDecimal(string attributeName) => GetAttribute<decimal>(attributeName);

    /// <summary>
    /// Get attribute as bool (convenience method)
    /// </summary>
    public bool? GetBool(string attributeName) => GetAttribute<bool>(attributeName);

    /// <summary>
    /// Get attribute as DateTime (convenience method)
    /// </summary>
    public DateTime? GetDateTime(string attributeName) => GetAttribute<DateTime>(attributeName);

    /// <summary>
    /// Get attribute as GeoPoint (convenience method)
    /// </summary>
    public GeoPoint? GetGeoPoint(string attributeName) => GetAttribute<GeoPoint>(attributeName);

    /// <summary>
    /// Convert to dictionary (for Neo4j storage)
    /// </summary>
    public Dictionary<string, object?> ToDictionary()
    {
        var dict = new Dictionary<string, object?>
        {
            ["id"] = Id,
            ["entityType"] = EntityType,
            ["tenantId"] = TenantId,
            ["domainId"] = DomainId,
            ["createdAt"] = Metadata.CreatedAt,
            ["updatedAt"] = Metadata.UpdatedAt,
            ["version"] = Metadata.Version
        };

        // Flatten attributes into root for easier Neo4j querying
        foreach (var (key, value) in Attributes)
        {
            dict[key] = value;
        }

        return dict;
    }

    /// <summary>
    /// Create from dictionary (for Neo4j retrieval)
    /// </summary>
    public static GenericEntity FromDictionary(Dictionary<string, object?> dict, List<string>? labels = null)
    {
        var entity = new GenericEntity
        {
            Id = dict.GetValueOrDefault("id")?.ToString() ?? Guid.NewGuid().ToString(),
            EntityType = dict.GetValueOrDefault("entityType")?.ToString() ?? "Unknown",
            TenantId = dict.GetValueOrDefault("tenantId")?.ToString() ?? "",
            DomainId = dict.GetValueOrDefault("domainId")?.ToString() ?? "",
            Labels = labels ?? new List<string>()
        };

        entity.Metadata = new EntityMetadata
        {
            CreatedAt = dict.GetValueOrDefault("createdAt") as DateTime? ?? DateTime.UtcNow,
            UpdatedAt = dict.GetValueOrDefault("updatedAt") as DateTime? ?? DateTime.UtcNow,
            Version = dict.GetValueOrDefault("version") as int? ?? 1
        };

        // All other properties go into Attributes
        var reservedKeys = new HashSet<string> { "id", "entityType", "tenantId", "domainId", "createdAt", "updatedAt", "version" };
        foreach (var (key, value) in dict)
        {
            if (!reservedKeys.Contains(key))
            {
                entity.Attributes[key] = value;
            }
        }

        return entity;
    }
}

/// <summary>
/// Entity metadata
/// </summary>
public class EntityMetadata
{
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("created_by")]
    public string? CreatedBy { get; set; }

    [JsonPropertyName("updated_by")]
    public string? UpdatedBy { get; set; }

    [JsonPropertyName("version")]
    public int Version { get; set; } = 1;

    [JsonPropertyName("is_deleted")]
    public bool IsDeleted { get; set; } = false;

    [JsonPropertyName("deleted_at")]
    public DateTime? DeletedAt { get; set; }
}

/// <summary>
/// Represents a relationship between two entities
/// </summary>
public class GenericRelationship
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("relationship_type")]
    public string RelationshipType { get; set; } = null!;

    [JsonPropertyName("from_entity_id")]
    public string FromEntityId { get; set; } = null!;

    [JsonPropertyName("to_entity_id")]
    public string ToEntityId { get; set; } = null!;

    [JsonPropertyName("from_entity_type")]
    public string FromEntityType { get; set; } = null!;

    [JsonPropertyName("to_entity_type")]
    public string ToEntityType { get; set; } = null!;

    [JsonPropertyName("properties")]
    public Dictionary<string, object?> Properties { get; set; } = new();

    [JsonPropertyName("metadata")]
    public EntityMetadata Metadata { get; set; } = new();

    [JsonPropertyName("tenant_id")]
    public string TenantId { get; set; } = null!;

    [JsonPropertyName("domain_id")]
    public string DomainId { get; set; } = null!;

    /// <summary>
    /// Get a property value with type safety
    /// </summary>
    public T? GetProperty<T>(string propertyName)
    {
        if (!Properties.TryGetValue(propertyName, out var value) || value == null)
        {
            return default;
        }

        try
        {
            if (value is T typedValue)
            {
                return typedValue;
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Set a property value
    /// </summary>
    public void SetProperty(string propertyName, object? value)
    {
        Properties[propertyName] = value;
    }
}

/// <summary>
/// Geographic point (latitude, longitude)
/// </summary>
public class GeoPoint
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("altitude")]
    public double? Altitude { get; set; }

    public GeoPoint() { }

    public GeoPoint(double latitude, double longitude, double? altitude = null)
    {
        Latitude = latitude;
        Longitude = longitude;
        Altitude = altitude;
    }

    public override string ToString() => Altitude.HasValue
        ? $"({Latitude}, {Longitude}, {Altitude})"
        : $"({Latitude}, {Longitude})";
}

/// <summary>
/// Geographic shape (polygon, line, etc.)
/// </summary>
public class GeoShape
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Polygon";  // Point, LineString, Polygon, MultiPolygon

    [JsonPropertyName("coordinates")]
    public List<List<GeoPoint>> Coordinates { get; set; } = new();

    [JsonPropertyName("properties")]
    public Dictionary<string, object?> Properties { get; set; } = new();
}

/// <summary>
/// Query filter for generic entities
/// </summary>
public class EntityFilter
{
    [JsonPropertyName("entity_type")]
    public string? EntityType { get; set; }

    [JsonPropertyName("tenant_id")]
    public string? TenantId { get; set; }

    [JsonPropertyName("domain_id")]
    public string? DomainId { get; set; }

    [JsonPropertyName("attribute_filters")]
    public Dictionary<string, AttributeFilter> AttributeFilters { get; set; } = new();

    [JsonPropertyName("search_query")]
    public string? SearchQuery { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; } = 100;

    [JsonPropertyName("offset")]
    public int Offset { get; set; } = 0;

    [JsonPropertyName("sort_by")]
    public string? SortBy { get; set; }

    [JsonPropertyName("sort_order")]
    public string SortOrder { get; set; } = "asc";
}

/// <summary>
/// Filter for entity attributes
/// </summary>
public class AttributeFilter
{
    [JsonPropertyName("operator")]
    public string Operator { get; set; } = "equals";  // equals, not_equals, contains, gt, lt, gte, lte, in, between

    [JsonPropertyName("value")]
    public object? Value { get; set; }

    [JsonPropertyName("values")]
    public List<object>? Values { get; set; }
}
