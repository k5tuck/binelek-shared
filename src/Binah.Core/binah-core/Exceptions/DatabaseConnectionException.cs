using System;
using Binah.Core.Constants;

namespace Binah.Core.Exceptions;

/// <summary>
/// Exception thrown when database connection fails
/// </summary>
public class DatabaseConnectionException : BinahException
{
    public string DatabaseName { get; }

    public DatabaseConnectionException(
        string databaseName,
        string reason,
        Exception? innerException = null)
        : base(
            $"Failed to connect to database '{databaseName}': {reason}",
            ErrorCodes.DatabaseConnectionFailed,
            innerException)
    {
        DatabaseName = databaseName;
        WithContext("databaseName", databaseName);
        WithContext("reason", reason);
    }
}
