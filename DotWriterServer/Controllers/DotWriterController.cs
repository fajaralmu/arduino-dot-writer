using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotWriterServer.Dto;
using DotWriterServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotWriterServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DotWriterController : Controller
    {
        private readonly IDotWriterService _dotWriterService;

        public DotWriterController(IDotWriterService dotWriterService)
        {
            _dotWriterService = dotWriterService;
        }

        [HttpPost, Route("execute")]
        public ActionResult<WebResponse<bool>> Execute([FromForm] string base64Image)
        {
            return Json(_dotWriterService.Execute(base64Image));
        }

        [HttpPost, Route("disconnect")]
        public ActionResult<WebResponse<bool>> Disconnect()
        {
            return Json(_dotWriterService.Disconnect());
        }
    }
}
