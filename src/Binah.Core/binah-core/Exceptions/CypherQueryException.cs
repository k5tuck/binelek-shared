using System;
using Binah.Core.Constants;

namespace Binah.Core.Exceptions;

/// <summary>
/// Exception thrown when a Cypher query execution fails
/// </summary>
public class CypherQueryException : BinahException
{
    public string Query { get; }

    public CypherQueryException(
        string query,
        string reason,
        Exception? innerException = null)
        : base(
            $"Failed to execute Cypher query: {reason}",
            ErrorCodes.QueryExecutionFailed,
            innerException)
    {
        Query = query;
        WithContext("query", query);
        WithContext("reason", reason);
    }
}
