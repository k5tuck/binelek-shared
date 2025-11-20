# Binah.Contracts

> **📚 For detailed API documentation, see [docs/libraries/binah-contracts.md](../../docs/libraries/binah-contracts.md)**

Shared contracts, DTOs, and event definitions for the Binelek platform.

## Overview

This library contains shared data transfer objects (DTOs), API contracts, and event schemas used across all Binelek microservices. It ensures consistent data structures and communication patterns.

## Contents

### DTOs
- **Ontology DTOs** - Entity and relationship data structures
  - `CreateEntityRequest`
  - `UpdateEntityRequest`
  - `EntityDto`
  - `RelationshipDto`
  - `QueryRequest`

### Events
- **Entity Events** - Domain events for entity lifecycle
  - `EntityCreatedEvent`
  - `EntityUpdatedEvent`
  - `EntityDeletedEvent`
  - `RelationshipCreatedEvent`
  - `OntologyEvent` (base class)

### Common Models
- `ApiResponse<T>` - Standardized API response wrapper
- `ErrorResponse` - Error response structure
- `PagedResult<T>` - Pagination wrapper

## Usage

### In .NET Projects

```bash
dotnet add reference ../../libraries/binah-contracts/Binah.Contracts.csproj
```

### Example Usage

```csharp
using Binah.Contracts.DTOs.Ontology;
using Binah.Contracts.Events;

// Using a DTO
var request = new CreateEntityRequest
{
    EntityType = "Property",
    Properties = new Dictionary<string, object>
    {
        { "address", "123 Main St" },
        { "price", 500000 }
    }
};

// Publishing an event
var entityCreatedEvent = new EntityCreatedEvent
{
    EntityId = "prop_123",
    EntityType = "Property",
    TenantId = "tenant_a",
    Timestamp = DateTime.UtcNow
};
```

## Dependencies

- None (this is a pure contract library)

## Versioning

- Current version: 1.0.0
- Follows semantic versioning
- Breaking changes require major version bump

## Related Libraries

- [binah-core](../binah-core/README.md) - Core utilities and exceptions
- [binah-infrastructure](../binah-infrastructure/README.md) - Infrastructure abstractions

## License

Internal use only - Binelek Platform
