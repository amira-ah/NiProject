using Final_project.Data.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Final_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FetchController : ControllerBase
    {
        public fetchService _FetchService;
        public FetchController(fetchService fetchservice)
        {
            _FetchService = fetchservice;
        }
        [HttpGet("get-testing")]
        public IActionResult GetRecords()
        {

            return Ok(_FetchService.getData());
            
        }
        [HttpGet("get-Daily")]
        public IActionResult GetDaily([FromQuery(Name = "getformat")] string getformat, [FromQuery(Name = "getDatefrom")] string getDatefrom, [FromQuery(Name = "getDateTo")] string getDateTo)
        {
            string getAggregation = HttpContext.Request.Query["getformat"].ToString();

            if (HttpContext.Request.Query["getDatefrom"].ToString() !="undefined" && HttpContext.Request.Query["getDateTo"].ToString() != "undefined")
            {

                return Ok(_FetchService.GetDataDate(HttpContext.Request.Query["getDatefrom"].ToString(), HttpContext.Request.Query["getDateTo"].ToString(), getAggregation));


            }
           

            if (getAggregation == "DailyData")
            {
              
                    return Ok(_FetchService.GetDailyData());
             

            }
            else { 
             
                    return Ok(_FetchService.getData());
                

            }
        }
       
         //   return Ok(_FetchService.GetDailyData());

        }
     /*   [HttpGet("get-Daily")]
        public IActionResult GetDaily([FromQuery(Name = "getformat")] string getformat, [FromQuery(Name = "getDatefrom")] string getDatefrom, [FromQuery(Name = "getDateTo")] string getDateTo)
        {
            string getAggregation = HttpContext.Request.Query["getformat"].ToString();
            if (getAggregation == "DailyData")
            {

                return Ok(_FetchService.GetDailyData());
            }
            else
            {
                return Ok(_FetchService.getData());
            }

            //   return Ok(_FetchService.GetDailyData());

        }*/
    /*    [HttpGet("get-Date")]
        public IActionResult GetDate([FromQuery(Name = "getDatefrom")] string getDatefrom, [FromQuery(Name = "getDateTo")] string getDateTo)
        {

       

            string DateFrom = HttpContext.Request.Query["getDatefrom"].ToString();
            string DateTo = HttpContext.Request.Query["getDateTo"].ToString();
           return Ok(_FetchService.GetDataDate(DateFrom, DateTo));
            //   return Ok(_FetchService.GetDailyData());

        }*/

    }

