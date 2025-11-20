using System;
using Binah.Core.Constants;

namespace Binah.Core.Exceptions;

/// <summary>
/// Exception thrown when event publishing fails
/// </summary>
public class EventPublishException : BinahException
{
    public string EventType { get; }

    public EventPublishException(
        string eventType,
        string reason,
        Exception? innerException = null)
        : base(
            $"Failed to publish event of type '{eventType}': {reason}",
            ErrorCodes.EventPublishFailed,
            innerException)
    {
        EventType = eventType;
        WithContext("eventType", eventType);
        WithContext("reason", reason);
    }
}
