using BookingIO.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingIO.Controllers;

public class WeatherController : BaseController
{
    private readonly WeatherService _weatherService;

    public WeatherController(WeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    public async Task<ActionResult> Current([FromQuery]string location)
    {
        var currentWeather = await _weatherService.GetCurrentWeather(location);

        if (currentWeather is null) return NotFound();

        return new JsonResult(currentWeather);
    }

    public async Task<ActionResult> Forecast([FromQuery]string location, [FromQuery]int days = 3)
    {
        var forecast = await _weatherService.GetForecast(location, days);
        if (forecast is null) return NotFound();
        return new JsonResult(forecast);
    }
}
