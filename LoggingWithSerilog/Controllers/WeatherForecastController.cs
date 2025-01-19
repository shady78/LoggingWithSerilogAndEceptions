using LoggingWithSerilog.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LoggingWithSerilog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<WeatherForecastController> _logger;

        private readonly WeatherOptions _weatherOptions;
        private readonly WeatherOptions _optionsSnapshot;
        private readonly WeatherOptions _optionsMonitor;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };


        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration configuration,
            IOptions<WeatherOptions> weatherOptions,
            IOptionsSnapshot<WeatherOptions> optionsSnapshot, 
            IOptionsMonitor<WeatherOptions> optionsMonitor)
        {
            _logger = logger;
            _configuration = configuration;
            _weatherOptions = weatherOptions.Value;
            _optionsSnapshot = optionsSnapshot.Value;
            _optionsMonitor = optionsMonitor.CurrentValue;
        }

        //[HttpGet(Name = "GetWeatherForecast")]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}


        [HttpGet("options")]
        public IActionResult GetFromOptionsPattern()
        {
            var response = new
            {
                options = new { _weatherOptions.City, _weatherOptions.State, _weatherOptions.Temperature, _weatherOptions.Summary },
                optionsSnapshot = new { _optionsSnapshot.City, _optionsSnapshot.State, _optionsSnapshot.Temperature, _optionsSnapshot.Summary },
                optionsMonitor = new { _optionsMonitor.City, _optionsMonitor.State, _optionsMonitor.Temperature, _optionsMonitor.Summary }
            };
            return Ok(response);
        }


        //[HttpGet("options")]
        //public IActionResult GetFromOptionsPattern()
        //{
        //    return Ok(new
        //    {
        //        _weatherOptions.City,
        //        _weatherOptions.State,
        //        _weatherOptions.Temperature,
        //        _weatherOptions.Summary
        //    });
        //}

        //[HttpGet("config")]
        //    public IActionResult GetTest()
        //    {
        //        var city = _configuration.GetValue<string>("WeatherOptions:City");
        //        var state = _configuration.GetValue<string>("WeatherOptions:State");
        //        var temperature = _configuration.GetValue<int>("WeatherOptions:Temperature");
        //        var summary = _configuration.GetValue<string>("WeatherOptions:Summary");
        //        return Ok(new
        //        {
        //            City = city,
        //            State = state,
        //            Temperature = temperature,
        //            Summary = summary
        //        });
        //    }
    }
    
}

