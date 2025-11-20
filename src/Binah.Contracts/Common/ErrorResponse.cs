using System;
using System.Collections.Generic;

namespace Binah.Contracts.Common;

/// <summary>
/// Error response structure
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Error code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Error details
    /// </summary>
    public Dictionary<string, object>? Details { get; set; }

    /// <summary>
    /// Timestamp of the error
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Request path where error occurred
    /// </summary>
    public string? Path { get; set; }
}
