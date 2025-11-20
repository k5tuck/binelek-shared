# Binah.Core

> **📚 For detailed API documentation, see [docs/libraries/binah-core.md](../../docs/libraries/binah-core.md)**

Core utilities, exceptions, and base classes for the Binelek platform.

## Overview

This library provides shared core functionality used across all Binelek services, including exception handling, logging utilities, validation, and common base classes.

## Contents

### Exceptions
- `BinahException` - Base exception for all Binelek exceptions
- `EntityNotFoundException` - Entity not found errors
- `ValidationException` - Data validation errors
- `UnauthorizedException` - Authorization errors
- `ConflictException` - Conflict errors

### Utilities
- **Logging** - Structured logging helpers
- **Validation** - Common validation rules
- **Extensions** - C# extension methods
- **Helpers** - General utility functions

### Base Classes
- `Entity` - Base class for domain entities
- `ValueObject` - Base class for value objects
- `AggregateRoot` - Domain-driven design aggregate root

## Usage

### In .NET Projects

```bash
dotnet add reference ../../libraries/binah-core/Binah.Core.csproj
```

### Example Usage

```csharp
using Binah.Core.Exceptions;
using Binah.Core.Extensions;

// Using custom exceptions
public async Task<Property> GetPropertyAsync(string id)
{
    var property = await _repository.FindByIdAsync(id);
    if (property == null)
    {
        throw new EntityNotFoundException($"Property with ID '{id}' not found");
    }
    return property;
}

// Using extensions
var trimmedValue = value.TrimOrDefault();
var isValidGuid = guidString.IsValidGuid();
```

## Dependencies

- Microsoft.Extensions.Logging
- System.ComponentModel.Annotations

## Related Libraries

- [binah-contracts](../binah-contracts/README.md) - Shared contracts and DTOs
- [binah-infrastructure](../binah-infrastructure/README.md) - Infrastructure abstractions

## License

Internal use only - Binelek Platform
