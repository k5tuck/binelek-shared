# Binah.Core.Domain

> **📚 For detailed API documentation, see [docs/libraries/binah-domain.md](../../docs/libraries/binah-domain.md)**

Shared library for domain-agnostic entity management in the Binelek platform. This library provides the core abstractions and services for working with generic entities across any industry vertical (real estate, healthcare, finance, smart cities, etc.).

## Overview

`Binah.Core.Domain` is the foundation of Binelek's multi-domain architecture. It provides:

- **Generic Entity Model**: Replace typed models (Property, Patient, etc.) with flexible entities
- **Dynamic GraphQL Schema Generation**: Auto-generate GraphQL schemas from domain ontology
- **Neo4j Integration**: Generic repository for graph database operations
- **Domain Registry Client**: Communicate with domain registry service
- **Type-Safe Attribute Access**: Work with dynamic attributes in a type-safe manner

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                      Application Layer                       │
│           (API Controllers, GraphQL Resolvers)               │
└──────────────────────────┬──────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────┐
│                  Binah.Core.Domain                           │
│  ┌────────────────┐  ┌────────────────┐  ┌───────────────┐ │
│  │ GenericEntity  │  │  DomainRegistry│  │    GraphQL    │ │
│  │    Service     │  │     Client     │  │Schema Builder │ │
│  └────────────────┘  └────────────────┘  └───────────────┘ │
└──────────────────────────┬──────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────┐
│                       Data Layer                             │
│              (Neo4j, PostgreSQL, Qdrant)                     │
└─────────────────────────────────────────────────────────────┘
```

## Core Components

### 1. GenericEntity

The `GenericEntity` class is the universal data container that can represent any domain object:

```csharp
public class GenericEntity
{
    public string Id { get; set; }
    public string EntityType { get; set; }  // "Property", "InfrastructureAsset", "Patient"
    public string TenantId { get; set; }
    public string DomainId { get; set; }    // "real-estate", "smart-cities", "healthcare"
    public Dictionary<string, object?> Attributes { get; set; }
    public EntityMetadata Metadata { get; set; }
    public List<string> Labels { get; set; }
}
```

**Example Usage:**

```csharp
// Real Estate Domain
var property = new GenericEntity
{
    EntityType = "Property",
    DomainId = "real-estate",
    TenantId = "acme-realty",
    Attributes = new Dictionary<string, object?>
    {
        ["address"] = "123 Main St",
        ["propertyType"] = "Commercial",
        ["sqft"] = 5000,
        ["listPrice"] = 500000
    }
};

// Smart Cities Domain
var infrastructure = new GenericEntity
{
    EntityType = "InfrastructureAsset",
    DomainId = "smart-cities",
    TenantId = "city-sf",
    Attributes = new Dictionary<string, object?>
    {
        ["assetType"] = "Bridge",
        ["condition"] = "Good",
        ["location"] = new GeoPoint(37.7749, -122.4194),
        ["yearBuilt"] = 1998
    }
};

// Healthcare Domain
var patient = new GenericEntity
{
    EntityType = "Patient",
    DomainId = "healthcare",
    TenantId = "general-hospital",
    Attributes = new Dictionary<string, object?>
    {
        ["firstName"] = "John",
        ["lastName"] = "Doe",
        ["dateOfBirth"] = new DateTime(1980, 1, 1),
        ["bloodType"] = "O+"
    }
};
```

**Type-Safe Attribute Access:**

```csharp
// Get attributes with type safety
var address = property.GetString("address");  // "123 Main St"
var sqft = property.GetInt("sqft");          // 5000
var price = property.GetDecimal("listPrice"); // 500000

var location = infrastructure.GetGeoPoint("location");
var yearBuilt = infrastructure.GetInt("yearBuilt");

// Set attributes
property.SetAttribute("status", "Active");
property.SetAttribute("lastUpdated", DateTime.UtcNow);
```

### 2. GenericEntityService

Service for CRUD operations on generic entities in Neo4j:

```csharp
public interface IGenericEntityService
{
    Task<GenericEntity> CreateAsync(GenericEntity entity);
    Task<GenericEntity?> GetByIdAsync(string id, string entityType, string tenantId);
    Task<List<GenericEntity>> QueryAsync(EntityFilter filter);
    Task<GenericEntity> UpdateAsync(GenericEntity entity);
    Task<bool> DeleteAsync(string id, string entityType, string tenantId, bool soft = true);
    Task<GenericRelationship> CreateRelationshipAsync(GenericRelationship relationship);
    Task<List<GenericRelationship>> GetRelationshipsAsync(string entityId, string relationshipType, string direction = "both");
    Task<int> CountAsync(EntityFilter filter);
}
```

**Example Usage:**

```csharp
// Inject service
private readonly IGenericEntityService _entityService;

// Create entity
var entity = new GenericEntity { /* ... */ };
var created = await _entityService.CreateAsync(entity);

// Query entities with filtering
var filter = new EntityFilter
{
    EntityType = "Property",
    TenantId = "acme-realty",
    DomainId = "real-estate",
    AttributeFilters = new Dictionary<string, AttributeFilter>
    {
        ["propertyType"] = new AttributeFilter
        {
            Operator = "equals",
            Value = "Commercial"
        },
        ["listPrice"] = new AttributeFilter
        {
            Operator = "lte",
            Value = 1000000
        }
    },
    Limit = 50,
    SortBy = "listPrice",
    SortOrder = "desc"
};

var properties = await _entityService.QueryAsync(filter);

// Get by ID
var property = await _entityService.GetByIdAsync(
    id: "prop123",
    entityType: "Property",
    tenantId: "acme-realty"
);

// Update entity
property.SetAttribute("status", "Sold");
property.SetAttribute("soldDate", DateTime.UtcNow);
var updated = await _entityService.UpdateAsync(property);

// Create relationship
var relationship = new GenericRelationship
{
    RelationshipType = "MONITORED_BY",
    FromEntityId = "bridge-001",
    ToEntityId = "sensor-042",
    FromEntityType = "InfrastructureAsset",
    ToEntityType = "IoTDevice",
    TenantId = "city-sf",
    DomainId = "smart-cities",
    Properties = new Dictionary<string, object?>
    {
        ["installDate"] = DateTime.UtcNow,
        ["alertThreshold"] = 0.8
    }
};

await _entityService.CreateRelationshipAsync(relationship);

// Get related entities
var sensors = await _entityService.GetRelationshipsAsync(
    entityId: "bridge-001",
    relationshipType: "MONITORED_BY",
    direction: "outgoing"
);
```

### 3. DynamicSchemaBuilder

Generate GraphQL schema from domain ontology:

```csharp
var schemaBuilder = new DynamicSchemaBuilder(
    domainId: "smart-cities",
    entities: domainOntology.Entities,
    relationships: domainOntology.Relationships
);

var graphQLSchema = schemaBuilder.GenerateSchema();
```

**Generated Schema Example:**

```graphql
type InfrastructureAsset {
  id: ID!
  entityType: String!
  tenantId: String!
  domainId: String!
  assetType: String!
  condition: String!
  location: GeoPoint!
  yearBuilt: Int
  estimatedValue: Float

  # Relationships
  iotDevices: [IoTDevice!]!

  createdAt: DateTime!
  updatedAt: DateTime!
  version: Int!
}

input InfrastructureAssetCreateInput {
  assetType: String!
  condition: String!
  location: GeoPoint!
  yearBuilt: Int
  estimatedValue: Float
}

input InfrastructureAssetFilterInput {
  AND: [InfrastructureAssetFilterInput!]
  OR: [InfrastructureAssetFilterInput!]
  assetType: StringFilterInput
  condition: StringFilterInput
  yearBuilt: NumberFilterInput
}

type Query {
  infrastructureAsset(id: ID!): InfrastructureAsset
  infrastructureAssets(
    filter: InfrastructureAssetFilterInput
    limit: Int = 100
    offset: Int = 0
    sortBy: String
    sortOrder: SortOrder = ASC
  ): [InfrastructureAsset!]!
  infrastructureAssetsCount(filter: InfrastructureAssetFilterInput): Int!
  searchInfrastructureAssets(query: String!, limit: Int = 20): [InfrastructureAsset!]!
}

type Mutation {
  createInfrastructureAsset(input: InfrastructureAssetCreateInput!): InfrastructureAsset!
  updateInfrastructureAsset(input: InfrastructureAssetUpdateInput!): InfrastructureAsset!
  deleteInfrastructureAsset(id: ID!, soft: Boolean = true): Boolean!
}
```

### 4. DomainRegistryClient

Client for communicating with the Domain Registry service:

```csharp
public interface IDomainRegistryClient
{
    Task<DomainInfo?> GetDomainAsync(string domainId);
    Task<List<EntityDefinition>> GetEntitiesAsync(string domainId);
    Task<EntityDefinition?> GetEntityDefinitionAsync(string domainId, string entityName);
    Task<List<DomainInfo>> GetAllDomainsAsync();
    Task<bool> ValidateDomainAsync(string domainId);
}
```

**Example Usage:**

```csharp
// Inject client
private readonly IDomainRegistryClient _domainClient;

// Get domain info
var domain = await _domainClient.GetDomainAsync("smart-cities");
Console.WriteLine($"Domain: {domain.Metadata.Name}");

// Get all entities for a domain
var entities = await _domainClient.GetEntitiesAsync("real-estate");
foreach (var entity in entities)
{
    Console.WriteLine($"Entity: {entity.Name}");
}

// Get specific entity definition
var entityDef = await _domainClient.GetEntityDefinitionAsync(
    "smart-cities",
    "InfrastructureAsset"
);

// Validate attributes against schema
foreach (var attr in entityDef.Attributes)
{
    Console.WriteLine($"{attr.Name}: {attr.Type} (Required: {attr.Required})");
}

// Validate domain
var isValid = await _domainClient.ValidateDomainAsync("healthcare");
```

## Setup and Configuration

### 1. Add Package Reference

```xml
<ItemGroup>
  <ProjectReference Include="..\..\shared\Binah.Core.Domain\Binah.Core.Domain.csproj" />
</ItemGroup>
```

### 2. Register Services

```csharp
using Binah.Core.Domain.Services;
using Neo4j.Driver;

// In Program.cs or Startup.cs
services.AddSingleton<IDriver>(sp =>
{
    var uri = configuration["Neo4j:Uri"];
    var user = configuration["Neo4j:Username"];
    var password = configuration["Neo4j:Password"];
    return GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
});

services.AddScoped<IGenericEntityService, GenericEntityService>();

services.AddMemoryCache();
services.AddHttpClient<IDomainRegistryClient, DomainRegistryClient>(client =>
{
    client.BaseAddress = new Uri(configuration["DomainRegistryUrl"] ?? "http://localhost:8100");
});
```

### 3. Use in Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class EntitiesController : ControllerBase
{
    private readonly IGenericEntityService _entityService;
    private readonly IDomainRegistryClient _domainClient;

    public EntitiesController(
        IGenericEntityService entityService,
        IDomainRegistryClient domainClient)
    {
        _entityService = entityService;
        _domainClient = domainClient;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEntityRequest request)
    {
        // Get entity definition to validate
        var entityDef = await _domainClient.GetEntityDefinitionAsync(
            request.DomainId,
            request.EntityType
        );

        if (entityDef == null)
        {
            return NotFound($"Entity type '{request.EntityType}' not found in domain '{request.DomainId}'");
        }

        // Create generic entity
        var entity = new GenericEntity
        {
            EntityType = request.EntityType,
            DomainId = request.DomainId,
            TenantId = User.FindFirst("tenantId")?.Value ?? "",
            Attributes = request.Attributes
        };

        // Save to Neo4j
        var created = await _entityService.CreateAsync(entity);

        return Ok(created);
    }

    [HttpGet]
    public async Task<IActionResult> Query([FromQuery] string entityType, [FromQuery] string domainId)
    {
        var filter = new EntityFilter
        {
            EntityType = entityType,
            DomainId = domainId,
            TenantId = User.FindFirst("tenantId")?.Value ?? ""
        };

        var entities = await _entityService.QueryAsync(filter);
        return Ok(entities);
    }
}
```

## Domain-Specific Examples

### Real Estate

```csharp
// Property
var property = new GenericEntity
{
    EntityType = "Property",
    DomainId = "real-estate",
    Attributes = new()
    {
        ["address"] = "123 Main St",
        ["propertyType"] = "Single Family",
        ["bedrooms"] = 3,
        ["bathrooms"] = 2.5,
        ["sqft"] = 2000,
        ["listPrice"] = 450000,
        ["yearBuilt"] = 2005
    }
};

// Owner
var owner = new GenericEntity
{
    EntityType = "Owner",
    DomainId = "real-estate",
    Attributes = new()
    {
        ["firstName"] = "Jane",
        ["lastName"] = "Smith",
        ["email"] = "jane@example.com",
        ["phone"] = "+1-555-0123"
    }
};

// Relationship: Owner OWNS Property
var ownership = new GenericRelationship
{
    RelationshipType = "OWNS",
    FromEntityId = owner.Id,
    ToEntityId = property.Id,
    Properties = new()
    {
        ["ownershipPercentage"] = 100,
        ["purchaseDate"] = new DateTime(2020, 6, 15)
    }
};
```

### Smart Cities

```csharp
// Infrastructure Asset
var bridge = new GenericEntity
{
    EntityType = "InfrastructureAsset",
    DomainId = "smart-cities",
    Attributes = new()
    {
        ["assetType"] = "Bridge",
        ["name"] = "Golden Gate Bridge",
        ["location"] = new GeoPoint(37.8199, -122.4783),
        ["condition"] = "Good",
        ["yearBuilt"] = 1937,
        ["lastInspectionDate"] = DateTime.UtcNow.AddMonths(-6)
    }
};

// IoT Device
var sensor = new GenericEntity
{
    EntityType = "IoTDevice",
    DomainId = "smart-cities",
    Attributes = new()
    {
        ["deviceType"] = "Vibration Sensor",
        ["manufacturer"] = "Acme Sensors",
        ["status"] = "Online",
        ["batteryLevel"] = 85,
        ["lastCommunication"] = DateTime.UtcNow
    }
};

// Relationship: Bridge MONITORED_BY Sensor
var monitoring = new GenericRelationship
{
    RelationshipType = "MONITORED_BY",
    FromEntityId = bridge.Id,
    ToEntityId = sensor.Id,
    Properties = new()
    {
        ["installDate"] = new DateTime(2023, 1, 15),
        ["sensorPosition"] = "North Tower, 50m"
    }
};
```

### Healthcare

```csharp
// Patient
var patient = new GenericEntity
{
    EntityType = "Patient",
    DomainId = "healthcare",
    Attributes = new()
    {
        ["mrn"] = "MRN-12345",
        ["firstName"] = "John",
        ["lastName"] = "Doe",
        ["dateOfBirth"] = new DateTime(1980, 5, 20),
        ["bloodType"] = "A+",
        ["allergies"] = new[] { "Penicillin", "Peanuts" }
    }
};

// Appointment
var appointment = new GenericEntity
{
    EntityType = "Appointment",
    DomainId = "healthcare",
    Attributes = new()
    {
        ["appointmentDate"] = new DateTime(2024, 2, 15, 14, 30, 0),
        ["appointmentType"] = "Annual Checkup",
        ["status"] = "Scheduled",
        ["notes"] = "Patient requested early morning slot"
    }
};

// Relationship: Patient HAS_APPOINTMENT Appointment
var patientAppointment = new GenericRelationship
{
    RelationshipType = "HAS_APPOINTMENT",
    FromEntityId = patient.Id,
    ToEntityId = appointment.Id
};
```

## Validation

Validate entity data against domain schema:

```csharp
public class EntityValidator
{
    private readonly IDomainRegistryClient _domainClient;

    public async Task<(bool IsValid, List<string> Errors)> ValidateAsync(GenericEntity entity)
    {
        var errors = new List<string>();

        // Get entity definition
        var entityDef = await _domainClient.GetEntityDefinitionAsync(
            entity.DomainId,
            entity.EntityType
        );

        if (entityDef == null)
        {
            return (false, new List<string> { $"Entity type '{entity.EntityType}' not found" });
        }

        // Validate required attributes
        foreach (var attr in entityDef.Attributes.Where(a => a.Required))
        {
            if (!entity.HasAttribute(attr.Name))
            {
                errors.Add($"Required attribute '{attr.Name}' is missing");
            }
        }

        // Validate attribute types
        foreach (var (attrName, attrValue) in entity.Attributes)
        {
            var attrDef = entityDef.Attributes.FirstOrDefault(a => a.Name == attrName);
            if (attrDef == null)
            {
                errors.Add($"Unknown attribute '{attrName}'");
                continue;
            }

            // Type validation (simplified)
            var isValid = ValidateAttributeType(attrValue, attrDef.Type);
            if (!isValid)
            {
                errors.Add($"Attribute '{attrName}' has invalid type (expected {attrDef.Type})");
            }
        }

        return (errors.Count == 0, errors);
    }

    private bool ValidateAttributeType(object? value, string expectedType)
    {
        if (value == null) return true;

        return expectedType.ToLower() switch
        {
            "string" => value is string,
            "number" => value is int or long or float or double or decimal,
            "integer" => value is int or long,
            "boolean" => value is bool,
            "date" or "datetime" => value is DateTime,
            "geo_point" => value is GeoPoint,
            _ => true  // Unknown types pass for now
        };
    }
}
```

## Benefits

1. **Domain Agnostic**: One codebase supports infinite industries
2. **Type Safety**: Generic methods with type parameters
3. **Graph Database**: Leverage Neo4j's relationship power
4. **Schema Validation**: Enforce data integrity via domain ontology
5. **GraphQL Auto-Gen**: No manual schema writing
6. **Multi-Tenancy**: Built-in tenant isolation
7. **Extensibility**: Easy to add new domains without code changes

## Future Enhancements

- [ ] Attribute-level encryption
- [ ] Complex validation rules (FluentValidation integration)
- [ ] Audit logging for all entity operations
- [ ] Event sourcing support
- [ ] Optimistic concurrency control
- [ ] Full-text search integration
- [ ] Geospatial query helpers

## Contributing

When extending this library:

1. Keep it domain-agnostic
2. Add tests for new features
3. Update this README
4. Follow existing patterns

## License

Proprietary - Binelek Platform
