using System;
using Binah.Core.Constants;

namespace Binah.Core.Exceptions;

/// <summary>
/// Exception thrown when inter-service communication fails
/// </summary>
public class ServiceCommunicationException : BinahException
{
    public string ServiceName { get; }

    public ServiceCommunicationException(
        string serviceName,
        string reason,
        Exception? innerException = null)
        : base(
            $"Failed to communicate with service '{serviceName}': {reason}",
            ErrorCodes.ServiceUnavailable,
            innerException)
    {
        ServiceName = serviceName;
        WithContext("serviceName", serviceName);
        WithContext("reason", reason);
    }
}
