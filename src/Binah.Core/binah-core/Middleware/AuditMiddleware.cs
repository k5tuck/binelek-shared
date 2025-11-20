using Binah.Core.Models;
using Binah.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Security.Claims;

namespace Binah.Core.Middleware;

/// <summary>
/// Middleware for automatic audit logging of HTTP requests
/// </summary>
public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditMiddleware> _logger;
    private static readonly HashSet<string> SensitiveFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "password", "token", "secret", "api_key", "apikey", "authorization",
        "access_token", "refresh_token", "client_secret", "private_key"
    };

    public AuditMiddleware(
        RequestDelegate next,
        ILogger<AuditMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context, IAuditService? auditService)
    {
        // Skip audit logging for health check and static files
        if (ShouldSkipAudit(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var originalBodyStream = context.Response.Body;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            // Only log if audit service is available
            if (auditService != null)
            {
                try
                {
                    await LogRequestAsync(context, stopwatch.ElapsedMilliseconds, auditService);
                }
                catch (Exception ex)
                {
                    // Don't let audit logging failure affect the request
                    _logger.LogError(ex, "Failed to log audit entry");
                }
            }
        }
    }

    private async Task LogRequestAsync(HttpContext context, long elapsedMs, IAuditService auditService)
    {
        var request = context.Request;
        var response = context.Response;

        // Extract user information from JWT claims
        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var tenantId = context.User?.FindFirst("tenant_id")?.Value ?? "system";

        // Determine action and resource from the request
        var (action, resource, resourceId) = ParseRequestInfo(request);

        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            Action = action,
            Resource = resource,
            ResourceId = resourceId,
            IpAddress = GetClientIpAddress(context),
            UserAgent = request.Headers["User-Agent"].ToString(),
            RequestPath = request.Path,
            HttpMethod = request.Method,
            StatusCode = response.StatusCode,
            Details = null, // Can be populated with additional context if needed
            Timestamp = DateTime.UtcNow
        };

        await auditService.LogActionAsync(auditLog);
    }

    private (string action, string? resource, string? resourceId) ParseRequestInfo(HttpRequest request)
    {
        var path = request.Path.Value ?? string.Empty;
        var method = request.Method;

        // Parse path to extract resource and ID
        // Example: /api/properties/123 -> resource: properties, resourceId: 123
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        string? resource = null;
        string? resourceId = null;

        if (segments.Length >= 2 && segments[0].Equals("api", StringComparison.OrdinalIgnoreCase))
        {
            resource = segments[1];
            if (segments.Length >= 3 && !string.IsNullOrEmpty(segments[2]))
            {
                // Check if segment is likely an ID (not another endpoint)
                if (Guid.TryParse(segments[2], out _) || int.TryParse(segments[2], out _))
                {
                    resourceId = segments[2];
                }
            }
        }

        // Determine action based on HTTP method and path
        var action = (method, resource) switch
        {
            ("POST", "auth") when path.Contains("login") => "user.login",
            ("POST", "auth") when path.Contains("register") => "user.register",
            ("POST", "auth") when path.Contains("logout") => "user.logout",
            ("POST", var r) when !string.IsNullOrEmpty(r) => $"{r}.create",
            ("PUT", var r) when !string.IsNullOrEmpty(r) => $"{r}.update",
            ("PATCH", var r) when !string.IsNullOrEmpty(r) => $"{r}.update",
            ("DELETE", var r) when !string.IsNullOrEmpty(r) => $"{r}.delete",
            ("GET", var r) when !string.IsNullOrEmpty(r) && !string.IsNullOrEmpty(resourceId) => $"{r}.view",
            ("GET", var r) when !string.IsNullOrEmpty(r) => $"{r}.list",
            _ => $"{method.ToLower()}.{path}"
        };

        return (action, resource, resourceId);
    }

    private string? GetClientIpAddress(HttpContext context)
    {
        // Check for forwarded IP addresses (common in load-balanced environments)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (ips.Length > 0)
            {
                return ips[0].Trim();
            }
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        return context.Connection.RemoteIpAddress?.ToString();
    }

    private bool ShouldSkipAudit(PathString path)
    {
        var pathValue = path.Value?.ToLowerInvariant() ?? string.Empty;

        return pathValue.Contains("/health") ||
               pathValue.Contains("/swagger") ||
               pathValue.Contains("/metrics") ||
               pathValue.EndsWith(".js") ||
               pathValue.EndsWith(".css") ||
               pathValue.EndsWith(".map") ||
               pathValue.EndsWith(".ico");
    }
}
