namespace Binah.Infrastructure.MultiTenancy;

/// <summary>
/// Provides access to the current licensee context in async flows
/// Stores licensee ID in AsyncLocal for thread-safe access
/// </summary>
public class LicenseeContext
{
    private static readonly AsyncLocal<Guid?> _licenseeId = new();

    /// <summary>
    /// Gets or sets the current licensee ID for this async context
    /// </summary>
    public static Guid? LicenseeId
    {
        get => _licenseeId.Value;
        set => _licenseeId.Value = value;
    }

    /// <summary>
    /// Get the current licensee ID or throw if not set
    /// </summary>
    public static Guid GetRequiredLicenseeId()
    {
        if (_licenseeId.Value == null)
        {
            throw new InvalidOperationException("Licensee context is not set. Ensure LicenseeContextMiddleware is registered.");
        }

        return _licenseeId.Value.Value;
    }

    /// <summary>
    /// Check if licensee context is set
    /// </summary>
    public static bool HasLicenseeContext()
    {
        return _licenseeId.Value != null;
    }

    /// <summary>
    /// Clear the licensee context
    /// </summary>
    public static void Clear()
    {
        _licenseeId.Value = null;
    }
}
