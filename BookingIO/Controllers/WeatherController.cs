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

    public async Task<ActionResult> Current()
    {
        var d = await _weatherService.GetCurrentWeather("-25.44583, -49.35921");
        return new OkResult();
    }
}
