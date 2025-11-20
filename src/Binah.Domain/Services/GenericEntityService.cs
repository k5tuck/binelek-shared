using Binah.Core.Domain.Models;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using System.Text;
using System.Text.Json;

namespace Binah.Core.Domain.Services;

public interface IGenericEntityService
{
    Task<GenericEntity> CreateAsync(GenericEntity entity);
    Task<GenericEntity?> GetByIdAsync(string id, string entityType, string tenantId);
    Task<List<GenericEntity>> QueryAsync(EntityFilter filter);
    Task<GenericEntity> UpdateAsync(GenericEntity entity);
    Task<bool> DeleteAsync(string id, string entityType, string tenantId, bool soft = true);
    Task<GenericRelationship> CreateRelationshipAsync(GenericRelationship relationship);
    Task<List<GenericRelationship>> GetRelationshipsAsync(string entityId, string relationshipType, string direction = "both");
    Task<bool> DeleteRelationshipAsync(string relationshipId);
    Task<int> CountAsync(EntityFilter filter);
}

public class GenericEntityService : IGenericEntityService
{
    private readonly IDriver _neo4jDriver;
    private readonly ILogger<GenericEntityService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public GenericEntityService(IDriver neo4jDriver, ILogger<GenericEntityService> logger)
    {
        _neo4jDriver = neo4jDriver;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<GenericEntity> CreateAsync(GenericEntity entity)
    {
        entity.Metadata.CreatedAt = DateTime.UtcNow;
        entity.Metadata.UpdatedAt = DateTime.UtcNow;
        entity.Metadata.Version = 1;

        // Ensure labels are set
        if (entity.Labels.Count == 0)
        {
            entity.Labels = new List<string> { "Entity", entity.EntityType };
        }

        var query = BuildCreateQuery(entity);

        await using var session = _neo4jDriver.AsyncSession();
        try
        {
            var result = await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query.Cypher, query.Parameters);
                var record = await cursor.SingleAsync();
                return record["entity"].As<Neo4j.Driver.INode>();
            });

            _logger.LogInformation(
                "Created entity {EntityType} with ID {EntityId} for tenant {TenantId}",
                entity.EntityType, entity.Id, entity.TenantId);

            return NodeToEntity(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to create entity {EntityType} with ID {EntityId}",
                entity.EntityType, entity.Id);
            throw;
        }
    }

    public async Task<GenericEntity?> GetByIdAsync(string id, string entityType, string tenantId)
    {
        var query = @"
            MATCH (e:Entity)
            WHERE e.id = $id AND e.entityType = $entityType AND e.tenantId = $tenantId AND (e.isDeleted IS NULL OR e.isDeleted = false)
            RETURN e as entity
        ";

        var parameters = new Dictionary<string, object?>
        {
            ["id"] = id,
            ["entityType"] = entityType,
            ["tenantId"] = tenantId
        };

        await using var session = _neo4jDriver.AsyncSession();
        try
        {
            var result = await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, parameters);
                if (await cursor.FetchAsync())
                {
                    return cursor.Current["entity"].As<Neo4j.Driver.INode>();
                }
                return null;
            });

            if (result == null)
            {
                return null;
            }

            return NodeToEntity(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to get entity {EntityType} with ID {EntityId}",
                entityType, id);
            throw;
        }
    }

    public async Task<List<GenericEntity>> QueryAsync(EntityFilter filter)
    {
        var queryBuilder = BuildQueryFromFilter(filter);

        await using var session = _neo4jDriver.AsyncSession();
        try
        {
            var results = await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(queryBuilder.Cypher, queryBuilder.Parameters);
                var entities = new List<GenericEntity>();

                await foreach (var record in cursor)
                {
                    var node = record["entity"].As<Neo4j.Driver.INode>();
                    entities.Add(NodeToEntity(node));
                }

                return entities;
            });

            _logger.LogInformation(
                "Queried {Count} entities of type {EntityType} for tenant {TenantId}",
                results.Count, filter.EntityType, filter.TenantId);

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to query entities with filter: {Filter}",
                JsonSerializer.Serialize(filter, _jsonOptions));
            throw;
        }
    }

    public async Task<GenericEntity> UpdateAsync(GenericEntity entity)
    {
        entity.Metadata.UpdatedAt = DateTime.UtcNow;
        entity.Metadata.Version++;

        var query = BuildUpdateQuery(entity);

        await using var session = _neo4jDriver.AsyncSession();
        try
        {
            var result = await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query.Cypher, query.Parameters);
                var record = await cursor.SingleAsync();
                return record["entity"].As<Neo4j.Driver.INode>();
            });

            _logger.LogInformation(
                "Updated entity {EntityType} with ID {EntityId} to version {Version}",
                entity.EntityType, entity.Id, entity.Metadata.Version);

            return NodeToEntity(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to update entity {EntityType} with ID {EntityId}",
                entity.EntityType, entity.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string id, string entityType, string tenantId, bool soft = true)
    {
        string query;
        if (soft)
        {
            query = @"
                MATCH (e:Entity)
                WHERE e.id = $id AND e.entityType = $entityType AND e.tenantId = $tenantId
                SET e.isDeleted = true, e.deletedAt = datetime()
                RETURN e
            ";
        }
        else
        {
            query = @"
                MATCH (e:Entity)
                WHERE e.id = $id AND e.entityType = $entityType AND e.tenantId = $tenantId
                DETACH DELETE e
                RETURN count(e) as deleted
            ";
        }

        var parameters = new Dictionary<string, object?>
        {
            ["id"] = id,
            ["entityType"] = entityType,
            ["tenantId"] = tenantId
        };

        await using var session = _neo4jDriver.AsyncSession();
        try
        {
            var result = await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, parameters);
                return await cursor.FetchAsync();
            });

            _logger.LogInformation(
                "{DeleteType} deleted entity {EntityType} with ID {EntityId}",
                soft ? "Soft" : "Hard", entityType, id);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to delete entity {EntityType} with ID {EntityId}",
                entityType, id);
            throw;
        }
    }

    public async Task<GenericRelationship> CreateRelationshipAsync(GenericRelationship relationship)
    {
        relationship.Metadata.CreatedAt = DateTime.UtcNow;
        relationship.Metadata.UpdatedAt = DateTime.UtcNow;
        relationship.Metadata.Version = 1;

        var query = @"
            MATCH (from:Entity {id: $fromId, tenantId: $tenantId})
            MATCH (to:Entity {id: $toId, tenantId: $tenantId})
            CREATE (from)-[r:RELATIONSHIP {
                id: $id,
                relationshipType: $relationshipType,
                tenantId: $tenantId,
                domainId: $domainId,
                createdAt: datetime(),
                updatedAt: datetime(),
                version: 1
            }]->(to)
            SET r += $properties
            RETURN r as relationship, from.id as fromId, to.id as toId
        ";

        var parameters = new Dictionary<string, object?>
        {
            ["id"] = relationship.Id,
            ["fromId"] = relationship.FromEntityId,
            ["toId"] = relationship.ToEntityId,
            ["relationshipType"] = relationship.RelationshipType,
            ["tenantId"] = relationship.TenantId,
            ["domainId"] = relationship.DomainId,
            ["properties"] = relationship.Properties
        };

        await using var session = _neo4jDriver.AsyncSession();
        try
        {
            await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, parameters);
                await cursor.ConsumeAsync();
            });

            _logger.LogInformation(
                "Created relationship {RelationshipType} from {FromId} to {ToId}",
                relationship.RelationshipType, relationship.FromEntityId, relationship.ToEntityId);

            return relationship;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to create relationship {RelationshipType}",
                relationship.RelationshipType);
            throw;
        }
    }

    public async Task<List<GenericRelationship>> GetRelationshipsAsync(
        string entityId,
        string relationshipType,
        string direction = "both")
    {
        var query = direction.ToLower() switch
        {
            "outgoing" => @"
                MATCH (e:Entity {id: $entityId})-[r:RELATIONSHIP {relationshipType: $relationshipType}]->(other:Entity)
                RETURN r as relationship, e.id as fromId, other.id as toId, e.entityType as fromType, other.entityType as toType
            ",
            "incoming" => @"
                MATCH (e:Entity {id: $entityId})<-[r:RELATIONSHIP {relationshipType: $relationshipType}]-(other:Entity)
                RETURN r as relationship, other.id as fromId, e.id as toId, other.entityType as fromType, e.entityType as toType
            ",
            _ => @"
                MATCH (e:Entity {id: $entityId})-[r:RELATIONSHIP {relationshipType: $relationshipType}]-(other:Entity)
                RETURN r as relationship, e.id as fromId, other.id as toId, e.entityType as fromType, other.entityType as toType
            "
        };

        var parameters = new Dictionary<string, object?>
        {
            ["entityId"] = entityId,
            ["relationshipType"] = relationshipType
        };

        await using var session = _neo4jDriver.AsyncSession();
        try
        {
            var results = await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, parameters);
                var relationships = new List<GenericRelationship>();

                await foreach (var record in cursor)
                {
                    var rel = record["relationship"].As<IRelationship>();
                    var relationship = new GenericRelationship
                    {
                        Id = rel.Properties["id"]?.ToString() ?? "",
                        RelationshipType = rel.Properties["relationshipType"]?.ToString() ?? "",
                        FromEntityId = record["fromId"].As<string>(),
                        ToEntityId = record["toId"].As<string>(),
                        FromEntityType = record["fromType"].As<string>(),
                        ToEntityType = record["toType"].As<string>(),
                        TenantId = rel.Properties["tenantId"]?.ToString() ?? "",
                        DomainId = rel.Properties["domainId"]?.ToString() ?? ""
                    };

                    // Extract properties
                    foreach (var (key, value) in rel.Properties)
                    {
                        if (!new[] { "id", "relationshipType", "tenantId", "domainId", "createdAt", "updatedAt", "version" }.Contains(key))
                        {
                            relationship.Properties[key] = value;
                        }
                    }

                    relationships.Add(relationship);
                }

                return relationships;
            });

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to get relationships for entity {EntityId}",
                entityId);
            throw;
        }
    }

    public async Task<bool> DeleteRelationshipAsync(string relationshipId)
    {
        var query = @"
            MATCH ()-[r:RELATIONSHIP {id: $relationshipId}]->()
            DELETE r
            RETURN count(r) as deleted
        ";

        var parameters = new Dictionary<string, object?>
        {
            ["relationshipId"] = relationshipId
        };

        await using var session = _neo4jDriver.AsyncSession();
        try
        {
            var result = await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, parameters);
                var record = await cursor.SingleAsync();
                return record["deleted"].As<int>() > 0;
            });

            _logger.LogInformation("Deleted relationship {RelationshipId}", relationshipId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete relationship {RelationshipId}", relationshipId);
            throw;
        }
    }

    public async Task<int> CountAsync(EntityFilter filter)
    {
        var queryBuilder = BuildQueryFromFilter(filter, countOnly: true);

        await using var session = _neo4jDriver.AsyncSession();
        try
        {
            var count = await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(queryBuilder.Cypher, queryBuilder.Parameters);
                var record = await cursor.SingleAsync();
                return record["count"].As<int>();
            });

            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to count entities with filter: {Filter}",
                JsonSerializer.Serialize(filter, _jsonOptions));
            throw;
        }
    }

    // Helper methods

    private (string Cypher, Dictionary<string, object?> Parameters) BuildCreateQuery(GenericEntity entity)
    {
        var labels = string.Join(":", entity.Labels);
        var properties = entity.ToDictionary();

        var cypher = $@"
            CREATE (e:{labels} $properties)
            RETURN e as entity
        ";

        return (cypher, new Dictionary<string, object?> { ["properties"] = properties });
    }

    private (string Cypher, Dictionary<string, object?> Parameters) BuildUpdateQuery(GenericEntity entity)
    {
        var properties = entity.ToDictionary();

        var cypher = @"
            MATCH (e:Entity {id: $id, tenantId: $tenantId})
            SET e = $properties
            RETURN e as entity
        ";

        var parameters = new Dictionary<string, object?>
        {
            ["id"] = entity.Id,
            ["tenantId"] = entity.TenantId,
            ["properties"] = properties
        };

        return (cypher, parameters);
    }

    private (string Cypher, Dictionary<string, object?> Parameters) BuildQueryFromFilter(EntityFilter filter, bool countOnly = false)
    {
        var whereConditions = new List<string>();
        var parameters = new Dictionary<string, object?>();

        // Entity type filter
        if (!string.IsNullOrEmpty(filter.EntityType))
        {
            whereConditions.Add("e.entityType = $entityType");
            parameters["entityType"] = filter.EntityType;
        }

        // Tenant filter
        if (!string.IsNullOrEmpty(filter.TenantId))
        {
            whereConditions.Add("e.tenantId = $tenantId");
            parameters["tenantId"] = filter.TenantId;
        }

        // Domain filter
        if (!string.IsNullOrEmpty(filter.DomainId))
        {
            whereConditions.Add("e.domainId = $domainId");
            parameters["domainId"] = filter.DomainId;
        }

        // Not deleted
        whereConditions.Add("(e.isDeleted IS NULL OR e.isDeleted = false)");

        // Attribute filters
        foreach (var (attrName, attrFilter) in filter.AttributeFilters)
        {
            var paramName = $"attr_{attrName}";
            var condition = attrFilter.Operator.ToLower() switch
            {
                "equals" => $"e.{attrName} = ${paramName}",
                "not_equals" => $"e.{attrName} <> ${paramName}",
                "contains" => $"e.{attrName} CONTAINS ${paramName}",
                "gt" => $"e.{attrName} > ${paramName}",
                "lt" => $"e.{attrName} < ${paramName}",
                "gte" => $"e.{attrName} >= ${paramName}",
                "lte" => $"e.{attrName} <= ${paramName}",
                "in" => $"e.{attrName} IN ${paramName}",
                _ => null
            };

            if (condition != null)
            {
                whereConditions.Add(condition);
                parameters[paramName] = attrFilter.Operator.ToLower() == "in"
                    ? attrFilter.Values
                    : attrFilter.Value;
            }
        }

        // Build query
        var whereClause = whereConditions.Any() ? $"WHERE {string.Join(" AND ", whereConditions)}" : "";

        string cypher;
        if (countOnly)
        {
            cypher = $@"
                MATCH (e:Entity)
                {whereClause}
                RETURN count(e) as count
            ";
        }
        else
        {
            var orderByClause = !string.IsNullOrEmpty(filter.SortBy)
                ? $"ORDER BY e.{filter.SortBy} {filter.SortOrder.ToUpper()}"
                : "";

            cypher = $@"
                MATCH (e:Entity)
                {whereClause}
                RETURN e as entity
                {orderByClause}
                SKIP $offset
                LIMIT $limit
            ";

            parameters["offset"] = filter.Offset;
            parameters["limit"] = filter.Limit;
        }

        return (cypher, parameters);
    }

    private GenericEntity NodeToEntity(Neo4j.Driver.INode node)
    {
        var dict = node.Properties.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value
        );

        return GenericEntity.FromDictionary(dict, node.Labels.ToList());
    }
}
