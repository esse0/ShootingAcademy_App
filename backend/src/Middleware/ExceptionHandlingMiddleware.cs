using System.Buffers.Text;
using System.Net;
using System.Text.Json;
using ShootingAcademy.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ShootingAcademy.Middleware
{
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            if (exception is ValidationException validationEx)
            {
                context.Response.StatusCode = 400;
                var validationResponse = new
                {
                    Error = true,
                    Message = "Ошибка валидации",
                    Code = 400,
                    Show = true,
                    ValidationErrors = validationEx.Errors
                };
                var json = JsonSerializer.Serialize(validationResponse);
                await context.Response.WriteAsync(json);
                return;
            }
            else if (exception is BaseException baseEx)
            {
                context.Response.StatusCode = baseEx.Code;
            }
            else
            {
                context.Response.StatusCode = 500;
            }

            var response = new
            {
                Error = true,
                Message = exception.Message,
                Code = exception is BaseException baseEx2 ? baseEx2.Code : 500,
                Show = exception is BaseException
            };
            var jsonDefault = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonDefault);
        }
    }

    public class ValidationException : BaseException
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(Dictionary<string, string[]> errors) 
            : base("Ошибка валидации", 400)
        {
            Errors = errors;
        }

        public static ValidationException FromModelState(ModelStateDictionary modelState)
        {
            var errors = modelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );
            return new ValidationException(errors);
        }

        public static ValidationException FromProblemDetails(ProblemDetails problemDetails)
        {
            if (problemDetails.Extensions.TryGetValue("errors", out var errorsObj) && 
                errorsObj is Dictionary<string, string[]> errors)
            {
                return new ValidationException(errors);
            }
            
            return new ValidationException(new Dictionary<string, string[]>
            {
                { "General", new[] { problemDetails.Title ?? "Ошибка валидации" } }
            });
        }
    }
} 