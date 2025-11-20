using Binah.Contracts.Models;
using Binah.Core.Domain.Models;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace Binah.Core.Domain.GraphQL;

/// <summary>
/// Builds dynamic GraphQL schema from domain ontology
/// </summary>
public class DynamicSchemaBuilder
{
    private readonly string _domainId;
    private readonly List<EntityDefinition> _entities;
    private readonly List<RelationshipDefinition>? _relationships;

    public DynamicSchemaBuilder(
        string domainId,
        List<EntityDefinition> entities,
        List<RelationshipDefinition>? relationships = null)
    {
        _domainId = domainId;
        _entities = entities;
        _relationships = relationships;
    }

    /// <summary>
    /// Generate GraphQL schema string
    /// </summary>
    public string GenerateSchema()
    {
        var schema = new StringBuilder();

        // Header
        schema.AppendLine($"# GraphQL Schema for Domain: {_domainId}");
        schema.AppendLine($"# Auto-generated from domain ontology");
        schema.AppendLine();

        // Generate type definitions for each entity
        foreach (var entity in _entities)
        {
            GenerateEntityType(schema, entity);
            schema.AppendLine();
        }

        // Generate input types for mutations
        foreach (var entity in _entities)
        {
            GenerateInputType(schema, entity, "Create");
            schema.AppendLine();
            GenerateInputType(schema, entity, "Update");
            schema.AppendLine();
        }

        // Generate filter input types
        foreach (var entity in _entities)
        {
            GenerateFilterInputType(schema, entity);
            schema.AppendLine();
        }

        // Generate relationship types
        if (_relationships != null)
        {
            foreach (var relationship in _relationships)
            {
                GenerateRelationshipType(schema, relationship);
                schema.AppendLine();
            }
        }

        // Generate Query type
        GenerateQueryType(schema);
        schema.AppendLine();

        // Generate Mutation type
        GenerateMutationType(schema);
        schema.AppendLine();

        // Generate common scalar types
        GenerateScalarTypes(schema);

        return schema.ToString();
    }

    private void GenerateEntityType(StringBuilder schema, EntityDefinition entity)
    {
        var description = entity.Description != null
            ? $"\"\"\"{entity.Description}\"\"\"\n"
            : "";

        schema.AppendLine($"{description}type {entity.Name} {{");
        schema.AppendLine("  id: ID!");
        schema.AppendLine("  entityType: String!");
        schema.AppendLine("  tenantId: String!");
        schema.AppendLine("  domainId: String!");

        foreach (var attr in entity.Attributes)
        {
            var graphQLType = MapToGraphQLType(attr);
            var attrDescription = attr.Description != null
                ? $"  \"\"\"{attr.Description}\"\"\"\n"
                : "";
            schema.AppendLine($"{attrDescription}  {attr.Name}: {graphQLType}");
        }

        // Add relationships as fields
        if (_relationships != null)
        {
            var outgoingRels = _relationships.Where(r => r.From == entity.Name).ToList();
            foreach (var rel in outgoingRels)
            {
                var isList = rel.Type == "one-to-many" || rel.Type == "many-to-many";
                var fieldName = ToCamelCase(rel.To) + (isList ? "s" : "");
                var returnType = isList ? $"[{rel.To}!]!" : rel.To;
                schema.AppendLine($"  {fieldName}: {returnType}");
            }

            var incomingRels = _relationships.Where(r => r.To == entity.Name).ToList();
            foreach (var rel in incomingRels)
            {
                var isList = rel.Type == "many-to-one" || rel.Type == "many-to-many";
                var fieldName = ToCamelCase(rel.From) + (isList ? "s" : "");
                var returnType = isList ? $"[{rel.From}!]!" : rel.From;
                schema.AppendLine($"  {fieldName}: {returnType}");
            }
        }

        // Metadata
        schema.AppendLine("  createdAt: DateTime!");
        schema.AppendLine("  updatedAt: DateTime!");
        schema.AppendLine("  version: Int!");

        schema.AppendLine("}");
    }

    private void GenerateInputType(StringBuilder schema, EntityDefinition entity, string operation)
    {
        schema.AppendLine($"input {entity.Name}{operation}Input {{");

        if (operation == "Update")
        {
            schema.AppendLine("  id: ID!");
        }

        foreach (var attr in entity.Attributes)
        {
            // Skip ID for create (auto-generated)
            if (operation == "Create" && attr.Name.ToLower() == "id")
            {
                continue;
            }

            var graphQLType = MapToGraphQLType(attr, isInput: true);

            // For update, make all fields optional
            if (operation == "Update" && attr.Required)
            {
                graphQLType = graphQLType.TrimEnd('!');
            }

            schema.AppendLine($"  {attr.Name}: {graphQLType}");
        }

        schema.AppendLine("}");
    }

    private void GenerateFilterInputType(StringBuilder schema, EntityDefinition entity)
    {
        schema.AppendLine($"input {entity.Name}FilterInput {{");
        schema.AppendLine("  AND: [" + entity.Name + "FilterInput!]");
        schema.AppendLine("  OR: [" + entity.Name + "FilterInput!]");

        foreach (var attr in entity.Attributes)
        {
            var filterType = MapToFilterType(attr.Type);
            if (filterType != null)
            {
                schema.AppendLine($"  {attr.Name}: {filterType}");
            }
        }

        schema.AppendLine("}");
    }

    private void GenerateRelationshipType(StringBuilder schema, RelationshipDefinition relationship)
    {
        schema.AppendLine($"type {relationship.Name}Relationship {{");
        schema.AppendLine("  id: ID!");
        schema.AppendLine($"  from: {relationship.From}!");
        schema.AppendLine($"  to: {relationship.To}!");
        schema.AppendLine("  relationshipType: String!");

        if (relationship.Properties != null)
        {
            foreach (var (key, value) in relationship.Properties)
            {
                schema.AppendLine($"  {key}: String");  // Default to String for flexibility
            }
        }

        schema.AppendLine("  createdAt: DateTime!");
        schema.AppendLine("  updatedAt: DateTime!");
        schema.AppendLine("}");
    }

    private void GenerateQueryType(StringBuilder schema)
    {
        schema.AppendLine("type Query {");

        foreach (var entity in _entities)
        {
            var entityNameLower = ToCamelCase(entity.Name);

            // Get by ID
            schema.AppendLine($"  {entityNameLower}(id: ID!): {entity.Name}");

            // List with filtering
            schema.AppendLine($"  {entityNameLower}s(");
            schema.AppendLine($"    filter: {entity.Name}FilterInput");
            schema.AppendLine("    limit: Int = 100");
            schema.AppendLine("    offset: Int = 0");
            schema.AppendLine("    sortBy: String");
            schema.AppendLine("    sortOrder: SortOrder = ASC");
            schema.AppendLine($"  ): [{entity.Name}!]!");

            // Count
            schema.AppendLine($"  {entityNameLower}sCount(filter: {entity.Name}FilterInput): Int!");

            // Search
            schema.AppendLine($"  search{entity.Name}s(query: String!, limit: Int = 20): [{entity.Name}!]!");
        }

        schema.AppendLine("}");
    }

    private void GenerateMutationType(StringBuilder schema)
    {
        schema.AppendLine("type Mutation {");

        foreach (var entity in _entities)
        {
            var entityNameLower = ToCamelCase(entity.Name);

            // Create
            schema.AppendLine($"  create{entity.Name}(input: {entity.Name}CreateInput!): {entity.Name}!");

            // Update
            schema.AppendLine($"  update{entity.Name}(input: {entity.Name}UpdateInput!): {entity.Name}!");

            // Delete
            schema.AppendLine($"  delete{entity.Name}(id: ID!, soft: Boolean = true): Boolean!");
        }

        // Relationship mutations
        if (_relationships != null)
        {
            schema.AppendLine();
            schema.AppendLine("  createRelationship(");
            schema.AppendLine("    fromId: ID!");
            schema.AppendLine("    toId: ID!");
            schema.AppendLine("    relationshipType: String!");
            schema.AppendLine("    properties: JSON");
            schema.AppendLine("  ): GenericRelationship!");

            schema.AppendLine();
            schema.AppendLine("  deleteRelationship(id: ID!): Boolean!");
        }

        schema.AppendLine("}");
    }

    private void GenerateScalarTypes(StringBuilder schema)
    {
        schema.AppendLine("# Custom scalar types");
        schema.AppendLine("scalar DateTime");
        schema.AppendLine("scalar JSON");
        schema.AppendLine("scalar GeoPoint");
        schema.AppendLine();
        schema.AppendLine("enum SortOrder {");
        schema.AppendLine("  ASC");
        schema.AppendLine("  DESC");
        schema.AppendLine("}");
        schema.AppendLine();
        schema.AppendLine("type GenericRelationship {");
        schema.AppendLine("  id: ID!");
        schema.AppendLine("  relationshipType: String!");
        schema.AppendLine("  fromEntityId: String!");
        schema.AppendLine("  toEntityId: String!");
        schema.AppendLine("  properties: JSON");
        schema.AppendLine("  createdAt: DateTime!");
        schema.AppendLine("}");
    }

    private string MapToGraphQLType(AttributeDefinition attr, bool isInput = false)
    {
        var baseType = attr.Type.ToLower() switch
        {
            "string" => "String",
            "text" => "String",
            "number" => "Float",
            "integer" => "Int",
            "boolean" => "Boolean",
            "date" => "DateTime",
            "datetime" => "DateTime",
            "email" => "String",
            "url" => "String",
            "phone" => "String",
            "enum" => attr.Name + "Enum",
            "json" => "JSON",
            "geo_point" => "GeoPoint",
            "geo_shape" => "JSON",
            "array" => "[String]",
            _ => "String"
        };

        // For enums, we'd need to generate enum definitions (todo)
        if (attr.Type.ToLower() == "enum" && attr.Values != null)
        {
            // This would require generating enum types separately
            baseType = "String";  // Fallback for now
        }

        // Add non-null modifier if required
        if (attr.Required && !isInput)
        {
            baseType += "!";
        }

        return baseType;
    }

    private string? MapToFilterType(string attributeType)
    {
        return attributeType.ToLower() switch
        {
            "string" => "StringFilterInput",
            "text" => "StringFilterInput",
            "number" => "NumberFilterInput",
            "integer" => "NumberFilterInput",
            "boolean" => "BooleanFilterInput",
            "date" => "DateTimeFilterInput",
            "datetime" => "DateTimeFilterInput",
            "enum" => "StringFilterInput",
            _ => null
        };
    }

    private string ToCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
        {
            return str;
        }

        return char.ToLowerInvariant(str[0]) + str.Substring(1);
    }
}

/// <summary>
/// Filter input types for GraphQL
/// </summary>
public class StringFilterInput
{
    public string? Equals { get; set; }
    public string? NotEquals { get; set; }
    public string? Contains { get; set; }
    public string? StartsWith { get; set; }
    public string? EndsWith { get; set; }
    public List<string>? In { get; set; }
    public List<string>? NotIn { get; set; }
}

public class NumberFilterInput
{
    public double? Equals { get; set; }
    public double? NotEquals { get; set; }
    public double? Gt { get; set; }
    public double? Gte { get; set; }
    public double? Lt { get; set; }
    public double? Lte { get; set; }
    public List<double>? In { get; set; }
}

public class BooleanFilterInput
{
    public bool? Equals { get; set; }
}

public class DateTimeFilterInput
{
    public DateTime? Equals { get; set; }
    public DateTime? Gt { get; set; }
    public DateTime? Gte { get; set; }
    public DateTime? Lt { get; set; }
    public DateTime? Lte { get; set; }
}
