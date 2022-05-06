using Final_project.Data.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Final_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class agreggatorController : ControllerBase
    {

        public static agreggatorService _agreggatorService;
        public agreggatorController(agreggatorService areggatorService)
        {
            _agreggatorService = areggatorService;
        }
        [HttpGet]
        public IActionResult Read_Iport()
        {
          //  _agreggatorService.aggregator();
            return Ok();
        }
       
        public static void OnCreated(object sender, FileSystemEventArgs e)
        {

          //  _agreggatorService.aggregator();
        }

    }
}
