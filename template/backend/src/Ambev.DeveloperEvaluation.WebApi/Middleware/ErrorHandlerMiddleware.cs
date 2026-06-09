using Ambev.DeveloperEvaluation.Domain.Exceptions;
using System.Net;
using System.Text.Json;
namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

public class ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception error)
        {
            logger.LogError(error, "Internal server error");

            var response = context.Response;
            response.ContentType = "application/json";

            string? errorMessage = error.Message ?? error.InnerException?.Message;
            object responseError;

            switch (error)
            {
                case SalesNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    responseError = new { Success = false, Message = error.Message ?? "Sale not found" };
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseError = new { Success = false, Message = "Something went wrong with your request. Try again later" };

                    break;
            }
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await response.WriteAsync(JsonSerializer.Serialize(responseError, jsonOptions));
        }
    }
}