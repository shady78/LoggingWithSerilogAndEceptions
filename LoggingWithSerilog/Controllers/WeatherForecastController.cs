using Microsoft.AspNetCore.Mvc;

namespace LoggingWithSerilog.Controllers
{
    [ApiController]
    [Route("[controller]")]
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

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        //[HttpGet("ThrowException")]
        //public IActionResult ThrowException()
        //{
        //    throw new Exception();
        //}

        //// Exception: Try-Catch
        //[HttpGet("GetTest")]
        //public IActionResult GetTest()
        //{
            
        //    try
        //    {
        //        var data = Get(); //Assume you get some data here which is also likely to throw an exception in certain cases.
        //        return Ok(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        return StatusCode(500);
        //    }
        //}
    }
}
