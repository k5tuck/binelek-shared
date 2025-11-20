# Binelek Shared Libraries

Shared .NET libraries used across all Binelek platform services.

## Libraries

### 1. Binah.Core
**Purpose:** Core utilities, exceptions, middleware, and extension methods

**Key Components:**
- `BinelekException` - Base exception hierarchy
- `Result<T>` - Result pattern for operation outcomes
- `ValidationMiddleware` - Global validation middleware
- Extension methods for common operations

**NuGet Package:** `Binah.Core`

### 2. Binah.Contracts
**Purpose:** DTOs, events, interfaces, and API contracts

**Key Components:**
- DTOs for all entities (EntityDto, PropertyDto, etc.)
- Kafka event schemas (EntityCreatedEvent, etc.)
- Service interfaces (IEntityService, IAuthService, etc.)
- API request/response models

**NuGet Package:** `Binah.Contracts`

### 3. Binah.Infrastructure
**Purpose:** Database contexts, repositories, configurations

**Key Components:**
- `DbContextBase` - Base database context
- Repository implementations (GenericRepository<T>)
- Kafka producer/consumer base classes
- Configuration helpers

**NuGet Package:** `Binah.Infrastructure`

### 4. Binah.Domain
**Purpose:** Domain models, value objects, aggregates

**Key Components:**
- Domain entities (Entity, Property, Owner, etc.)
- Value objects (Address, Money, etc.)
- Domain events
- Business logic

**NuGet Package:** `Binah.Domain`

## Installation

```bash
dotnet add package Binah.Core
dotnet add package Binah.Contracts
dotnet add package Binah.Infrastructure
dotnet add package Binah.Domain
```

## Development

### Build All Libraries
```bash
dotnet build
```

### Run Tests
```bash
dotnet test
```

### Publish to NuGet
```bash
dotnet pack --configuration Release
dotnet nuget push **/*.nupkg --source https://api.nuget.org/v3/index.json
```

## Usage Example

```csharp
using Binah.Core.Exceptions;
using Binah.Contracts.DTOs;
using Binah.Infrastructure.Repositories;

public class MyService
{
    private readonly IRepository<Entity> _repository;
    
    public async Task<Result<EntityDto>> GetEntityAsync(string id)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return Result<EntityDto>.Failure("Entity not found");
                
            return Result<EntityDto>.Success(entity.ToDto());
        }
        catch (BinelekException ex)
        {
            return Result<EntityDto>.Failure(ex.Message);
        }
    }
}
```

## Version Compatibility

| Library Version | .NET Version | Used By |
|----------------|--------------|---------|
| 1.0.x | .NET 8.0 | binelek-core, binelek-data, binelek-ai, binelek-billing |

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for development guidelines.

## License

MIT License - See [LICENSE](LICENSE)
