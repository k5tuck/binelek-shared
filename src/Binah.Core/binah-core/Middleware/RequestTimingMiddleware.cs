using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Binah.Core.Middleware;

/// <summary>
/// Middleware to measure and log request execution time
/// </summary>
public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;

    public RequestTimingMiddleware(
        RequestDelegate next,
        ILogger<RequestTimingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var correlationId = context.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            var logLevel = GetLogLevel(stopwatch.ElapsedMilliseconds, context.Response.StatusCode);

            _logger.Log(
                logLevel,
                "Request {Method} {Path} completed with status {StatusCode} in {ElapsedMs}ms. CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                correlationId);

            // Add timing header to response
            context.Response.Headers.Add("X-Response-Time-Ms", stopwatch.ElapsedMilliseconds.ToString());
        }
    }

    private LogLevel GetLogLevel(long elapsedMs, int statusCode)
    {
        // Log as warning if request takes too long or fails
        if (elapsedMs > 5000 || statusCode >= 500)
            return LogLevel.Warning;

        if (statusCode >= 400)
            return LogLevel.Information;

        // Log as information for successful requests
        return LogLevel.Information;
    }
}
