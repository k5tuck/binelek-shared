using System.Collections.Generic;

namespace Binah.Core.Extensions;

/// <summary>
/// Extension methods for Dictionary
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Get value or default
    /// </summary>
    public static TValue? GetValueOrDefault<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        TValue? defaultValue = default) where TKey : notnull
    {
        return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
    }

    /// <summary>
    /// Add or update a value in the dictionary
    /// </summary>
    public static void AddOrUpdate<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        TValue value) where TKey : notnull
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] = value;
        }
        else
        {
            dictionary.Add(key, value);
        }
    }

    /// <summary>
    /// Merge two dictionaries
    /// </summary>
    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> other,
        bool overwrite = true) where TKey : notnull
    {
        foreach (var kvp in other)
        {
            if (overwrite || !dictionary.ContainsKey(kvp.Key))
            {
                dictionary[kvp.Key] = kvp.Value;
            }
        }

        return dictionary;
    }
}
