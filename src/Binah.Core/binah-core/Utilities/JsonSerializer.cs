using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Binah.Core.Utilities;

/// <summary>
/// JSON serialization utilities
/// </summary>
public static class JsonSerializer
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    private static readonly JsonSerializerOptions PrettyOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>
    /// Serialize object to JSON string
    /// </summary>
    public static string Serialize<T>(T obj, bool pretty = false)
    {
        var options = pretty ? PrettyOptions : DefaultOptions;
        return System.Text.Json.JsonSerializer.Serialize(obj, options);
    }

    /// <summary>
    /// Deserialize JSON string to object
    /// </summary>
    public static T? Deserialize<T>(string json)
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(json, DefaultOptions);
    }

    /// <summary>
    /// Try to deserialize JSON string to object
    /// </summary>
    public static bool TryDeserialize<T>(string json, out T? result)
    {
        try
        {
            result = Deserialize<T>(json);
            return result != null;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}
