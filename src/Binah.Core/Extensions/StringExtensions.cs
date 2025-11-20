using System;
using System.Text.RegularExpressions;

namespace Binah.Core.Extensions;

/// <summary>
/// Extension methods for strings
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Check if a string is null or whitespace
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Convert string to PascalCase
    /// </summary>
    public static string ToPascalCase(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        var words = value.Split(new[] { '_', ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
        var result = string.Empty;

        foreach (var word in words)
        {
            if (word.Length > 0)
            {
                result += char.ToUpper(word[0]) + word.Substring(1).ToLower();
            }
        }

        return result;
    }

    /// <summary>
    /// Convert string to snake_case
    /// </summary>
    public static string ToSnakeCase(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        return Regex.Replace(
            Regex.Replace(value, @"([A-Z])([A-Z][a-z])", "$1_$2"),
            @"([a-z\d])([A-Z])", "$1_$2")
            .ToLower();
    }

    /// <summary>
    /// Truncate string to specified length
    /// </summary>
    public static string Truncate(this string value, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length <= maxLength)
            return value;

        return value.Substring(0, maxLength - suffix.Length) + suffix;
    }
}
