using System;
using Binah.Core.Constants;

namespace Binah.Core.Exceptions;

/// <summary>
/// Exception thrown when entity update fails
/// </summary>
public class EntityUpdateException : BinahException
{
    public string EntityId { get; }

    public EntityUpdateException(
        string entityId,
        string reason,
        Exception? innerException = null)
        : base(
            $"Failed to update entity '{entityId}': {reason}",
            ErrorCodes.EntityUpdateFailed,
            innerException)
    {
        EntityId = entityId;
        WithContext("entityId", entityId);
        WithContext("reason", reason);
    }
}
