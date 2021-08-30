using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotAlert.Controllers
{
    [Route("[Controller]")]
    public class HealthController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("I am alive");
        }
    }
}
