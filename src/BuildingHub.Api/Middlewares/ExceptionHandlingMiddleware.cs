
using BuildingHub.Modules.Tenancy.Application.Exceptions;
using BuildingHub.SharedKernel.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

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

            if (ex is ConcurrencyConflictException ||
                ex is DbUpdateConcurrencyException)
            {
                _logger.LogWarning(
                    ex,
                    "Concurrency conflict occurred.");
            }
            else
            {
                _logger.LogError(
                    ex,
                    "An exception occurred while processing the request.");
            }

            context.Response.Clear();
            context.Response.ContentType = "application/problem+json";

            var duplicateMessage = GetDuplicateMessage(ex);

            var tenantConflictMessage =
                ex is TenantHasCommunitiesException
                    ? ex.Message
                    : null;

            var conflictMessage =
                duplicateMessage ?? tenantConflictMessage;

            var isConcurrencyConflict =
                ex is ConcurrencyConflictException ||
                ex is DbUpdateConcurrencyException;

            var statusCode = ex switch
            {
                ConcurrencyConflictException =>
                    StatusCodes.Status409Conflict,

                DbUpdateConcurrencyException =>
                    StatusCodes.Status409Conflict,

                _ when conflictMessage is not null =>
                    StatusCodes.Status409Conflict,

                ArgumentException =>
                    StatusCodes.Status400BadRequest,

                InvalidOperationException =>
                    StatusCodes.Status400BadRequest,

                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = statusCode;

            var detail = isConcurrencyConflict
                ? "The record was modified by another request. " +
                  "Please reload and try again."
                : conflictMessage
                  ?? (statusCode == StatusCodes.Status400BadRequest
                      ? ex.Message
                      : "خطای غیرمنتظره‌ای در سرور رخ داده است.");

            var title = statusCode switch
            {
                StatusCodes.Status400BadRequest =>
                    "Request validation failed",

                StatusCodes.Status409Conflict
                    when isConcurrencyConflict =>
                    "Concurrency Conflict",

                StatusCodes.Status409Conflict
                    when duplicateMessage is not null =>
                    "Duplicate record",

                StatusCodes.Status409Conflict =>
                    "Conflict",

                _ => "Internal server error"
            };

            var response = new
            {
                title,
                status = statusCode,
                detail,
                traceId = context.TraceIdentifier
            };

            await context.Response.WriteAsJsonAsync(
                response,
                cancellationToken: context.RequestAborted);
        }
    }

    private static string? GetDuplicateMessage(
        Exception exception)
    {
        var postgresException = exception switch
        {
            DbUpdateException dbUpdateException =>
                dbUpdateException.GetBaseException()
                    as PostgresException,

            PostgresException directPostgresException =>
                directPostgresException,

            _ => null
        };

        if (postgresException?.SqlState !=
            PostgresErrorCodes.UniqueViolation)
        {
            return null;
        }

        return postgresException.ConstraintName switch
        {
            "IX_Floors_BuildingId_FloorNumber" =>
                "این شماره طبقه قبلاً برای این ساختمان ثبت شده است. " +
                "لطفاً شماره دیگری انتخاب کنید.",

            "IX_Units_BuildingId_UnitNumber" =>
                "این شماره واحد قبلاً برای این ساختمان ثبت شده است. " +
                "لطفاً شماره دیگری انتخاب کنید.",

            _ => null
        };
    }
}
