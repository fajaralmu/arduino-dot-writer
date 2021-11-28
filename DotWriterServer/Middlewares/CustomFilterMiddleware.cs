using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DotWriterServer.Middlewares
{
    public class CustomFilterMiddleware
    {
        private readonly RequestDelegate _next;
        public CustomFilterMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            if (context.Request.Path.StartsWithSegments("/assets"))
            {
                await _next(context);
                return;
            }
            Debug.WriteLine($" {context.Request.Method} : {context.Request.Path}{context.Request.QueryString}");
            long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            PopulateResponseForCors(context.Response);

            if (!context.Request.Method.ToLower().Equals("options"))
            {
                await _next(context);
            }
            long duration = DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
            double seconds = Math.Round((double)duration / 1000, 4);
            Debug.WriteLine(" - ResponseCode:" + context.Response.StatusCode + " TIME: " + seconds + " seconds");

        }

        public static void PopulateResponseForCors(HttpResponse response)
        {
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Expose-Headers", "access-token, requestid");
            response.Headers.Add("Access-Control-Allow-Credentials", "true");
            response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS, DELETE");
            response.Headers.Add("Access-Control-Max-Age", "3600");
            response.Headers.Add("Access-Control-Allow-Headers",
                "Content-Type, Accept, X-Requested-With, Authorization, requestid, access-token");
            response.StatusCode = StatusCodes.Status200OK;
        }

    }
}
