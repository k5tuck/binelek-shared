using Binah.Contracts.Common;
using Binah.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Binah.Core.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error on {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest, "VALIDATION_ERROR");
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access on {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex, HttpStatusCode.Unauthorized, "UNAUTHORIZED");
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found on {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex, HttpStatusCode.NotFound, "NOT_FOUND");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation on {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest, "INVALID_OPERATION");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception on {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError, "INTERNAL_ERROR");
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode, string errorCode)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var correlationId = context.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();

        var response = ApiResponse<object>.Fail(new ErrorResponse
        {
            Code = errorCode,
            Message = exception.Message,
            Path = context.Request.Path,
            Details = new Dictionary<string, object>
            {
                { "correlationId", correlationId },
                { "timestamp", DateTime.UtcNow }
            }
        });

        await JsonSerializer.SerializeAsync(context.Response.Body, response);
    }
}
