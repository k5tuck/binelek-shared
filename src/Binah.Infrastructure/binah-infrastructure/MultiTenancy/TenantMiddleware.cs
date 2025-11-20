using Microsoft.AspNetCore.Http;

namespace Binah.Infrastructure.MultiTenancy;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Extract tenant from header or query parameter (route values not available without routing package)
        var tenantId = context.Request.Headers["X-Tenant-Id"].FirstOrDefault()
                    ?? context.Request.Query["tenantId"].FirstOrDefault();

        if (!string.IsNullOrEmpty(tenantId) && Guid.TryParse(tenantId, out var parsedTenantId))
        {
            TenantContext.TenantId = parsedTenantId;
        }

        await _next(context);
    }
}
