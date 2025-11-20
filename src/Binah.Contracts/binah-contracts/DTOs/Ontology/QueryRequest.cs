using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Binah.Contracts.DTOs.Ontology;

/// <summary>
/// Request to execute a Cypher query
/// </summary>
public class QueryRequest
{
    /// <summary>
    /// Cypher query to execute
    /// </summary>
    [Required(ErrorMessage = "Query is required")]
    [StringLength(10000, ErrorMessage = "Query must not exceed 10,000 characters")]
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// Query parameters
    /// </summary>
    public Dictionary<string, object>? Parameters { get; set; }
}

/// <summary>
/// Result of a Cypher query execution
/// </summary>
public class QueryResult
{
    /// <summary>
    /// Query results as list of dictionaries
    /// </summary>
    public List<Dictionary<string, object>> Results { get; set; } = new();

    /// <summary>
    /// Number of results returned
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Execution time in milliseconds
    /// </summary>
    public long ExecutionTimeMs { get; set; }
}
