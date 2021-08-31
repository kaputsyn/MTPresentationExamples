using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IRequestClient<IGetWeather> _requestClient;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IRequestClient<IGetWeather> requestClient)
    {
        _logger = logger;
        _requestClient = requestClient;
    }

    [HttpGet]
    [Produces(typeof(IWeatherResponse))]
    public async Task<IActionResult> Get(string location)
    {
        var response = await _requestClient.GetResponse<IWeatherNotAwailable, IWeatherResponse>(new
        {
            Location = location
        });

        if (response.Is<IWeatherNotAwailable>(out var notAwailable)) 
        {
            return NotFound(notAwailable.Message.Reason);
        }
        if (response.Is<IWeatherResponse>(out var weather))
        {
            return Ok(weather.Message);
        }
        else 
        {
            return BadRequest();
        }
    }
}
