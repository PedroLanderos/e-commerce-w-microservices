using Llaveremos.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Llaveremos.SharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            string message = "Internal server error";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string tittle = "Error";

            try
            {
                await next(context);
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    tittle = "Warning";
                    message = "Too many request";
                    statusCode = (int)StatusCodes.Status429TooManyRequests;
                    await ModifyHeader(context, tittle, message, statusCode);
                }
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    tittle = "Alert";
                    message = "Client with no authorization";
                    statusCode = (int)StatusCodes.Status401Unauthorized;
                    await ModifyHeader(context, tittle, message, statusCode);
                }
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    tittle = "No access";
                    message = "You don't have access to this";
                    statusCode = (int)StatusCodes.Status403Forbidden;
                    await ModifyHeader(context, tittle, message, statusCode);
                }
            }
            catch (Exception ex)
            {

                LogException.LogExceptions(ex);
                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    tittle = "out of time";
                    message = "Request time out";
                    statusCode = (int)StatusCodes.Status408RequestTimeout;
                }
                await ModifyHeader(context, tittle, message, statusCode);
            }

        }

        private static async Task ModifyHeader(HttpContext context, string tittle, string message, int statusCode)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Detail = message,
                Status = statusCode,
                Title = tittle
            }), CancellationToken.None);
            return;
        }
    }
}
