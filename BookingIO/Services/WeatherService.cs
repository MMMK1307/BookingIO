using BookingIO.Models;

namespace BookingIO.Services;

public class WeatherService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WeatherService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<WeatherCurrentRequest?> GetCurrentWeather(string latlang)
    {
        string url = $"https://api.weatherapi.com/v1/current.json?q={latlang}&key=80115a74b97f4aa69f5112313261610";
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetFromJsonAsync<WeatherCurrentRequest>(url);
            return response;
        } catch(Exception)
        {
            return null;
        }
    }

}
