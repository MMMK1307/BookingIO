using BookingIO.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingIO.Controllers;

public class WeatherController
{
    private readonly WeatherService _weatherService;

    public WeatherController(WeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    public async Task<ActionResult> Current([FromQuery]string location)
    {
        return new JsonResult(await _weatherService.GetCurrentWeather(location));
    }

    public async Task<ActionResult> Forecast([FromQuery]string location, [FromQuery]int days = 3)
    {
        var forecast = await _weatherService.GetForecast(location, days);
        return new JsonResult(forecast);
    }
}
