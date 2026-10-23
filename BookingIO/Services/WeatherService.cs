using BookingIO.Models;

namespace BookingIO.Services;

public class WeatherService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WeatherService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private string _url(string path) => $"https://api.weatherapi.com/v1/{path}?key=80115a74b97f4aa69f5112313261610";

    public async Task<WeatherForecastReques?> GetCurrentWeather(string latlang)
    {
        string url = $"{_url("current.json")}&q={latlang}";
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetFromJsonAsync<WeatherForecastReques>(url);
            return response;
        } catch(Exception)
        {
            return null;
        }
    }

    public async Task<WeatherForecastRequest?> GetForecast(string latlang, int days)
    {
        string url = $"{_url("forecast.json")}&q={latlang}&days={days}";
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetFromJsonAsync<WeatherForecastRequest>(url);
            return response;
        } catch(Exception)
        {
            return null;
        }
    }

}
