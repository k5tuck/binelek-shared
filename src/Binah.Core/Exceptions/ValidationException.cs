using System.Collections.Generic;
using Binah.Core.Constants;

namespace Binah.Core.Exceptions;

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public class ValidationException : BinahException
{
    public Dictionary<string, List<string>> Errors { get; }

    public ValidationException(
        Dictionary<string, List<string>> errors)
        : base(
            "Validation failed",
            ErrorCodes.ValidationFailed)
    {
        Errors = errors;
        WithContext("errors", errors);
    }

    public ValidationException(string field, string errorMessage)
        : this(new Dictionary<string, List<string>>
        {
            { field, new List<string> { errorMessage } }
        })
    {
    }

    public ValidationException(string errorMessage)
        : this("general", errorMessage)
    {
    }
}
