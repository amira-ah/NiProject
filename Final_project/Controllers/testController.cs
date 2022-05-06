using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Threading.Tasks;

namespace Final_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class testController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public testController(IConfiguration config)
        {
            configuration = config;
        }

        [HttpGet("get-testing")]
        public IActionResult GetAuthorWithBooks()
        {


            string conString = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString(configuration, "VerticaConnectionstring");
            string queryString = "SELECT DISTINCT EMPLOYEE_NAME FROM employee";

            using (OdbcConnection connection = new OdbcConnection(conString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                // Execute the DataReader and access the data.
                OdbcDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("CustomerID={0}", reader[0]);
                    
                }

                // Call Close when done reading.
                reader.Close();
                return Ok();

            }
        }

  
    }
}
