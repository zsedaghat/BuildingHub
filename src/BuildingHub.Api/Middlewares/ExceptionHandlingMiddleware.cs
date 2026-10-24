
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingHub.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
                throw;

            _logger.LogError(
                ex,
                "An unhandled exception occurred while processing the request.");

            context.Response.Clear();
            context.Response.ContentType = "application/problem+json";

            var statusCode = ex switch
            {
                ArgumentException => StatusCodes.Status400BadRequest,
                InvalidOperationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = statusCode;

            var detail = statusCode == StatusCodes.Status400BadRequest
                ? ex.Message
                : "خطای غیرمنتظره‌ای در سرور رخ داده است.";

            var response = new
            {
                title = statusCode == StatusCodes.Status400BadRequest
                    ? "Request validation failed"
                    : "Internal server error",
                status = statusCode,
                detail,
                traceId = context.TraceIdentifier
            };

            await context.Response.WriteAsJsonAsync(
                response,
                cancellationToken: context.RequestAborted);
        }
    }
}
