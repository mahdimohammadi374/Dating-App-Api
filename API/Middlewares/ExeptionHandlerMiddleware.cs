using API.Errors;
using System.Net;
using System.Text.Json;

namespace API.Middlewares
{
    public class ExeptionHandlerMiddleware
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILoggerFactory _logger;
        private readonly RequestDelegate _next;

        public ExeptionHandlerMiddleware(IWebHostEnvironment env, ILoggerFactory logger, RequestDelegate next)
        {
            _env = env;
            _logger = logger;
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
                var logger = _logger.CreateLogger("ExceptionHandlerMiddleware");
                logger.LogError(ex, ex.Message);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var response = _env.IsDevelopment()
                    ? new ApiExeption(
                        (int)HttpStatusCode.InternalServerError,
                        ex.Message
                        , ex.StackTrace.ToString())
                    : new ApiExeption(
                         (int)HttpStatusCode.InternalServerError,
                        ex.Message);
                var options=new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json=JsonSerializer.Serialize(response , options);
                await context.Response.WriteAsync(json);

            }
        }

    }
}
