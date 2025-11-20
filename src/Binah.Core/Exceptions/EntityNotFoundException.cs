using Binah.Core.Constants;

namespace Binah.Core.Exceptions;

/// <summary>
/// Exception thrown when an entity is not found
/// </summary>
public class EntityNotFoundException : BinahException
{
    public string EntityId { get; }
    public string? EntityType { get; }

    public EntityNotFoundException(
        string entityId,
        string? entityType = null)
        : base(
            $"Entity '{entityId}' {(entityType != null ? $"of type '{entityType}'" : "")} not found",
            ErrorCodes.EntityNotFound)
    {
        EntityId = entityId;
        EntityType = entityType;
        WithContext("entityId", entityId);
        if (entityType != null)
        {
            WithContext("entityType", entityType);
        }
    }
}
