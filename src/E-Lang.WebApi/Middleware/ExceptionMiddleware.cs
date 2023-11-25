using E_Lang.Application.Common.Errors;
using System.Net;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace E_Lang.WebApi.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            
            var response = GetApiException(ex, context);

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = response.StatusCode;
            await context.Response.WriteAsync(json);
        }
    }

    private ApiException GetApiException(Exception exception, HttpContext context)
    {
        if (exception is CustomException ex)
        {
            var response = ex.ToApiException();

            if (_env.IsDevelopment())
            {
                response.Details = exception.StackTrace?.ToString();
            }

            return response;
        }

        return _env.IsDevelopment()
                ? new ApiException(context.Response.StatusCode, exception.Message, exception.StackTrace?.ToString())
                : new ApiException(context.Response.StatusCode, "Internal Server error");
    } 
}