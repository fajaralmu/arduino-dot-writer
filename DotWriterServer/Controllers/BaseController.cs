using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DotWriterServer.Controllers
{
    [Controller]
    public class BaseController : Controller
    {
        [HttpGet, Route("web")]
        public ViewResult Index()
         {
            ViewBag.Title = "Dot Writer";
            return View("index");
        }

    }
}
