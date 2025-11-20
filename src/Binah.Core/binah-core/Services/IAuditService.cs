using Binah.Core.Models;

namespace Binah.Core.Services;

/// <summary>
/// Interface for audit logging service
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Log an action to the audit trail
    /// </summary>
    /// <param name="log">Audit log entry</param>
    /// <returns>Task</returns>
    Task LogActionAsync(AuditLog log);

    /// <summary>
    /// Get audit logs with optional filtering
    /// </summary>
    /// <param name="tenantId">Tenant identifier</param>
    /// <param name="userId">Optional user filter</param>
    /// <param name="action">Optional action filter</param>
    /// <param name="resource">Optional resource filter</param>
    /// <param name="resourceId">Optional resource ID filter</param>
    /// <param name="startDate">Optional start date filter</param>
    /// <param name="endDate">Optional end date filter</param>
    /// <param name="skip">Number of records to skip</param>
    /// <param name="take">Number of records to take</param>
    /// <returns>List of audit logs</returns>
    Task<(List<AuditLog> logs, int totalCount)> GetAuditLogsAsync(
        string tenantId,
        string? userId = null,
        string? action = null,
        string? resource = null,
        string? resourceId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int skip = 0,
        int take = 50);

    /// <summary>
    /// Get user-specific activity logs
    /// </summary>
    /// <param name="tenantId">Tenant identifier</param>
    /// <param name="userId">User identifier</param>
    /// <param name="skip">Number of records to skip</param>
    /// <param name="take">Number of records to take</param>
    /// <returns>List of user activity logs</returns>
    Task<List<AuditLog>> GetUserActivityAsync(string tenantId, string userId, int skip = 0, int take = 50);

    /// <summary>
    /// Get history for a specific resource
    /// </summary>
    /// <param name="tenantId">Tenant identifier</param>
    /// <param name="resource">Resource type</param>
    /// <param name="resourceId">Resource identifier</param>
    /// <param name="skip">Number of records to skip</param>
    /// <param name="take">Number of records to take</param>
    /// <returns>List of resource history logs</returns>
    Task<List<AuditLog>> GetResourceHistoryAsync(string tenantId, string resource, string resourceId, int skip = 0, int take = 50);

    /// <summary>
    /// Export audit logs to CSV format
    /// </summary>
    /// <param name="tenantId">Tenant identifier</param>
    /// <param name="startDate">Start date for export</param>
    /// <param name="endDate">End date for export</param>
    /// <returns>CSV content as byte array</returns>
    Task<byte[]> ExportAuditLogsAsync(string tenantId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Get audit statistics for a tenant
    /// </summary>
    /// <param name="tenantId">Tenant identifier</param>
    /// <param name="startDate">Optional start date</param>
    /// <param name="endDate">Optional end date</param>
    /// <returns>Audit statistics</returns>
    Task<AuditStats> GetAuditStatsAsync(string tenantId, DateTime? startDate = null, DateTime? endDate = null);
}

/// <summary>
/// Audit statistics model
/// </summary>
public class AuditStats
{
    public int TotalActions { get; set; }
    public int UniqueUsers { get; set; }
    public Dictionary<string, int> ActionCounts { get; set; } = new();
    public Dictionary<string, int> ResourceCounts { get; set; } = new();
    public Dictionary<string, int> DailyActivity { get; set; } = new();
}
