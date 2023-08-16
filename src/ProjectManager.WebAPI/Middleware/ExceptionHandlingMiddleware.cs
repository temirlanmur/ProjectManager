using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Application.Exceptions;
using System.Diagnostics;
using System.Net;

namespace ProjectManager.WebAPI.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private const string DefaultErrorDetail = "An error occurred while processing your request.";
    private const string ValidationErrorDetail = "One or more validation errors have occured.";

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        (int httpStatusCode, string detail) = exception switch
        {
            BaseApplicationException ex => (ex.HttpStatusCode, ex.Message),
            ValidationException => ((int)HttpStatusCode.BadRequest, ValidationErrorDetail),
            _ => ((int)HttpStatusCode.InternalServerError, DefaultErrorDetail)
        };

        (string type, string title) = Defaults[httpStatusCode];

        ProblemDetails problemDetails = CreateProblemDetails(context, exception, type, title, httpStatusCode, detail);

        context.Response.StatusCode = httpStatusCode;
        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private ProblemDetails CreateProblemDetails(
        HttpContext context,
        Exception exception,
        string type,
        string title,
        int status,
        string detail)
    {
        ProblemDetails problemDetails = new()
        {
            Type = type,
            Title = title,
            Status = status,
            Detail = detail,
        };

        string traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        if (traceId != null)
        {
            problemDetails.Extensions.Add("traceId", traceId);
        }

        if (exception is ValidationException ex)
        {
            Dictionary<string, string> validationErrors = ex.Errors.ToDictionary(e => e.PropertyName, e => e.ErrorMessage);
            problemDetails.Extensions.Add("validationErrors", validationErrors);
        }

        return problemDetails;
    }

    private static readonly Dictionary<int, (string type, string title)> Defaults = new()
    {
        [400] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            "Bad Request"
        ),

        [401] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.2",
            "Unauthorized"
        ),

        [403] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.4",
            "Forbidden"
        ),

        [404] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            "Not Found"
        ),

        [405] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.6",
            "Method Not Allowed"
        ),

        [406] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.7",
            "Not Acceptable"
        ),

        [408] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.9",
            "Request Timeout"
        ),

        [409] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.10",
            "Conflict"
        ),

        [412] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.13",
            "Precondition Failed"
        ),

        [415] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.16",
            "Unsupported Media Type"
        ),

        [422] =
        (
            "https://tools.ietf.org/html/rfc4918#section-11.2",
            "Unprocessable Entity"
        ),

        [426] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.22",
            "Upgrade Required"
        ),

        [500] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.6.1",
            "Internal Server Error"
        ),

        [502] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.6.3",
            "Bad Gateway"
        ),

        [503] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.6.4",
            "Service Unavailable"
        ),

        [504] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.6.5",
            "Gateway Timeout"
        ),
    };
}
