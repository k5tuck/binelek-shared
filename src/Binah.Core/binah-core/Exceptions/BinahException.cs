using System;
using System.Collections.Generic;

namespace Binah.Core.Exceptions;

/// <summary>
/// Base exception for all Binah platform exceptions
/// </summary>
public abstract class BinahException : Exception
{
    /// <summary>
    /// Error code for categorization and handling
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Additional context for the error
    /// </summary>
    public Dictionary<string, object> Context { get; }

    protected BinahException(string message, string errorCode, Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        Context = new Dictionary<string, object>();
    }

    /// <summary>
    /// Add context information to the exception
    /// </summary>
    public BinahException WithContext(string key, object value)
    {
        Context[key] = value;
        return this;
    }
}
