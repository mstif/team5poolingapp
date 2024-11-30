
using Services.Implementation.Exceptions;
using System.Net;
using System.Text.Json;

namespace api.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;   
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = string.Empty;

            switch (exception)
            {
                case NotFoundException:
                    code = HttpStatusCode.NotFound;
                    break;
                case NotExistException:
                    code = HttpStatusCode.BadRequest;
                    break;
                case AccessDeniedException:
                    code = HttpStatusCode.Forbidden;
                    break;
                case EntityLockedException:
                    code = HttpStatusCode.Locked;
                    break;              
                case BadRequestException:
                    code = HttpStatusCode.BadRequest;
                    break;
                case PValidationException:
                    code = HttpStatusCode.BadRequest;
                    break;
                default: 
                    code = HttpStatusCode.BadRequest;
                    break;


                    //case ValidationException validationException:
                    //    code = HttpStatusCode.BadRequest;
                    //    result = JsonSerializer.Serialize(validationException.Errors);
                    //    break;
            }

            _logger.Error(exception, "Something bad was happen response status code - {@code}",code);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            if (result == string.Empty)
            {
                result = JsonSerializer.Serialize( new
                {
                    StatusCode = (int)code,
                    ErrorMsg = exception.Message 
                });
            }

            return context.Response.WriteAsync(result);
        }
    }
}
