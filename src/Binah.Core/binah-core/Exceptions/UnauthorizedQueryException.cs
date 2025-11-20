using System.Collections.Generic;
using Binah.Core.Constants;

namespace Binah.Core.Exceptions;

/// <summary>
/// Exception thrown when a Cypher query is unauthorized or invalid
/// </summary>
public class UnauthorizedQueryException : BinahException
{
    public string Query { get; }
    public List<string> ViolatedRules { get; }

    public UnauthorizedQueryException(
        string query,
        List<string> violatedRules)
        : base(
            $"Query contains unauthorized operations: {string.Join(", ", violatedRules)}",
            ErrorCodes.UnauthorizedQuery)
    {
        Query = query;
        ViolatedRules = violatedRules;
        WithContext("query", query);
        WithContext("violatedRules", violatedRules);
    }
}
