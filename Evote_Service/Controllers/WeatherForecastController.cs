using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var req = Request;
            String remoteIpAddress = req.HttpContext.Connection.RemoteIpAddress.ToString();
            remoteIpAddress = remoteIpAddress.Split(":")[3];

            String forwardIP = Request.Headers["x-forwarded-for"];
            forwardIP= forwardIP.Split(',')[0];
            String dataTest = DateTime.Now.ToString() + " " + remoteIpAddress+" "+ forwardIP+ " "+ Environment.GetEnvironmentVariable("GATEWAY_IP"); ;

       
            return Ok(dataTest);
            //return Ok(Environment.GetEnvironmentVariable("CONNECTIONSTRINGS"));
        }
    }
}
