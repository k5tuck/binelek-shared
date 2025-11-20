namespace Binah.Core.Models;

/// <summary>
/// Audit log entity for tracking all critical user and system actions
/// </summary>
public class AuditLog
{
    /// <summary>
    /// Unique identifier for the audit log entry
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Tenant identifier for multi-tenancy support
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// User identifier who performed the action
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Action performed (e.g., "user.login", "property.create")
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Resource type affected (e.g., "properties", "users")
    /// </summary>
    public string? Resource { get; set; }

    /// <summary>
    /// Specific resource identifier
    /// </summary>
    public string? ResourceId { get; set; }

    /// <summary>
    /// IP address of the request
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent string from the request
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Request path
    /// </summary>
    public string? RequestPath { get; set; }

    /// <summary>
    /// HTTP method (GET, POST, PUT, DELETE, etc.)
    /// </summary>
    public string? HttpMethod { get; set; }

    /// <summary>
    /// HTTP status code of the response
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Additional details in JSON format
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Timestamp when the action occurred
    /// </summary>
    public DateTime Timestamp { get; set; }
}
