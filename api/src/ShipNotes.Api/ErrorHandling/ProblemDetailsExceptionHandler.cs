using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ShipNotes.Application.Common.Exceptions;
using ShipNotes.Domain.Common;

namespace ShipNotes.Api.ErrorHandling;

/// <summary>Maps application exceptions to RFC 9457 problem details responses.</summary>
internal sealed class ProblemDetailsExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails? problem = exception switch
        {
            ValidationException validation => CreateValidationProblem(validation),
            NotFoundException => new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "Not found", Detail = exception.Message },
            ConflictException => new ProblemDetails { Status = StatusCodes.Status409Conflict, Title = "Conflict", Detail = exception.Message },
            DomainException => new ProblemDetails { Status = StatusCodes.Status400BadRequest, Title = "Invalid request", Detail = exception.Message },
            _ => null,
        };

        if (problem is null)
        {
            return false;
        }

        httpContext.Response.StatusCode = problem.Status!.Value;
        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
            Exception = exception,
        });
    }

    private static ValidationProblemDetails CreateValidationProblem(ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(e => ToCamelCase(e.PropertyName))
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        return new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation errors occurred.",
        };
    }

    private static string ToCamelCase(string name) =>
        string.IsNullOrEmpty(name) ? name : char.ToLowerInvariant(name[0]) + name[1..];
}
