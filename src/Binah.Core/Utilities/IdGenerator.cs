using System;
using System.Linq;

namespace Binah.Core.Utilities;

/// <summary>
/// Generates unique identifiers for entities
/// </summary>
public static class IdGenerator
{
    private static readonly Random Random = new();

    /// <summary>
    /// Generate a unique ID with optional prefix
    /// </summary>
    /// <param name="prefix">Optional prefix (e.g., "proj", "task")</param>
    /// <returns>Unique identifier</returns>
    public static string Generate(string? prefix = null)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var randomPart = GenerateRandomString(8);

        return prefix != null
            ? $"{prefix}-{timestamp}-{randomPart}"
            : $"{timestamp}-{randomPart}";
    }

    /// <summary>
    /// Generate a GUID-based ID
    /// </summary>
    public static string GenerateGuid()
    {
        return Guid.NewGuid().ToString("N"); // Without hyphens
    }

    /// <summary>
    /// Generate a short ID (8 characters)
    /// </summary>
    public static string GenerateShort()
    {
        return GenerateRandomString(8);
    }

    private static string GenerateRandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
}
