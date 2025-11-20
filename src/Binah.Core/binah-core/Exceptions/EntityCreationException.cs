using System;
using Binah.Core.Constants;

namespace Binah.Core.Exceptions;

/// <summary>
/// Exception thrown when entity creation fails
/// </summary>
public class EntityCreationException : BinahException
{
    public string EntityType { get; }

    public EntityCreationException(
        string entityType,
        string reason,
        Exception? innerException = null)
        : base(
            $"Failed to create entity of type '{entityType}': {reason}",
            ErrorCodes.EntityCreationFailed,
            innerException)
    {
        EntityType = entityType;
        WithContext("entityType", entityType);
        WithContext("reason", reason);
    }
}
