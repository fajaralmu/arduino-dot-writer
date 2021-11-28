using DotWriterServer.Dto;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DotWriterServer.Controllers
{
    [ApiController]
    public class ErrorController : Controller
    {
        [HttpGet, HttpDelete, HttpPut, HttpOptions, HttpPost]
        [Route("/error")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public ActionResult<WebResponse<string>> ErrorLocalDevelopment(
           [FromServices] IWebHostEnvironment webHostEnvironment)
        {
            // if (webHostEnvironment.EnvironmentName != "Development")
            // {
            //     throw new InvalidOperationException(
            //         "This shouldn't be invoked in non-development environments.");
            // }

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return Problem(
                     type: context.Error.GetType().Name,
                     detail: context.Error.StackTrace,
                     title: context.Error.Message
                   );
        }

    }
}
