using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Llaveremos.SharedLibrary.Middleware
{
    public class ValidateApiGateway(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var header = context.Request.Headers["Api-Gateway"];
            if (header.FirstOrDefault() == null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("The request was not sent by the ApiGateway");
                return;
            }
            else
            {
                await next(context);
            }

        }

    }
}
