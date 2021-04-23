using eshop_api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Net;
using System.Threading.Tasks;

namespace eshop_api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionHandlingMiddleware(RequestDelegate request)
        {
            next = request;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {

                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            int statusCode;
            string detail;
            string title;

            var exceptionType = ex.GetType();
            if (exceptionType == typeof(EntityNotFoundException))
            {
                statusCode = 404;
                title = "Resource Not Found";
                detail = ex.Message;
            }
            else if (exceptionType == typeof(EntityAlreadyExistExcption))
            {
                statusCode = 401;
                title = "Resource Already Exist";
                detail = ex.Message;
            }
            else
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                Log.Error(ex, $"Something went wrong: {feature?.Error}");

                statusCode = 500;
                title = "Internal Server Error";
                detail = "Something went wrong. Please try again later.";
            }

            var response = new ApiError(statusCode, title, detail).ToString();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsync(response);
        }
    }
}
