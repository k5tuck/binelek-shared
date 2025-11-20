# Binah.Infrastructure

> **📚 For detailed API documentation, see [docs/libraries/binah-infrastructure.md](../../docs/libraries/binah-infrastructure.md)**

Infrastructure abstractions and implementations for the Binelek platform.

## Overview

This library provides infrastructure abstractions and base implementations for data access, messaging, caching, and external service integrations.

## Contents

### Data Access
- **Repository Pattern** - Generic repository interfaces
- **Unit of Work** - Transaction management
- **Connection Factories** - Database connection management

### Messaging
- **Kafka Abstractions** - Producer and consumer interfaces
- **Event Bus** - Domain event publishing
- **Message Serialization** - JSON, Avro, Protobuf

### Caching
- **Cache Manager** - Distributed caching abstraction
- **Redis Integration** - Redis client wrapper
- **In-Memory Cache** - Development caching

### External Services
- **Neo4j Client** - Graph database client
- **PostgreSQL Client** - Relational database client
- **Qdrant Client** - Vector database client
- **Elasticsearch Client** - Search engine client

## Usage

### In .NET Projects

```bash
dotnet add reference ../../libraries/binah-infrastructure/Binah.Infrastructure.csproj
```

### Example Usage

```csharp
using Binah.Infrastructure.Data;
using Binah.Infrastructure.Messaging;

// Using repository
public class PropertyService
{
    private readonly IRepository<Property> _repository;
    
    public PropertyService(IRepository<Property> repository)
    {
        _repository = repository;
    }
    
    public async Task<Property> CreateAsync(Property property)
    {
        return await _repository.AddAsync(property);
    }
}

// Using Kafka producer
public class EventPublisher
{
    private readonly IKafkaProducer _producer;
    
    public async Task PublishAsync(EntityCreatedEvent @event)
    {
        await _producer.ProduceAsync("ontology.entity.created.v1", @event);
    }
}
```

## Dependencies

- Binah.Core
- Neo4j.Driver
- Npgsql
- Confluent.Kafka
- StackExchange.Redis
- NEST (Elasticsearch)

## Configuration

Services using this library should configure:

```json
{
  "ConnectionStrings": {
    "Neo4j": "bolt://localhost:7687",
    "PostgreSQL": "Host=localhost;Database=binah;Username=user;Password=pass"
  },
  "Kafka": {
    "BootstrapServers": "localhost:9092"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  }
}
```

## Related Libraries

- [binah-contracts](../binah-contracts/README.md) - Shared contracts and DTOs
- [binah-core](../binah-core/README.md) - Core utilities and exceptions

## License

Internal use only - Binelek Platform
