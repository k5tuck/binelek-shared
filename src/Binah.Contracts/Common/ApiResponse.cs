using System.Collections.Generic;

namespace Binah.Contracts.Common;

/// <summary>
/// Generic API response wrapper
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates success or failure
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response data
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Error information (if Success is false)
    /// </summary>
    public ErrorResponse? Error { get; set; }

    /// <summary>
    /// Response metadata
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// Create a successful response
    /// </summary>
    public static ApiResponse<T> Ok(T data, Dictionary<string, object>? metadata = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Metadata = metadata
        };
    }

    /// <summary>
    /// Create a failed response
    /// </summary>
    public static ApiResponse<T> Fail(ErrorResponse error)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Error = error
        };
    }

    /// <summary>
    /// Create a failed response with a simple error message
    /// </summary>
    public static ApiResponse<T> WithError(string message, string code = "ERROR")
    {
        return Fail(new ErrorResponse
        {
            Code = code,
            Message = message
        });
    }
}
