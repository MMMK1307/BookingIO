using System.Web;
using BookingIO.Common.ConfigOption;
using BookingIO.Models;
using Microsoft.Extensions.Options;

namespace BookingIO.Services;

public class WeatherService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly WeatherOption _weatherOption;

    public WeatherService(IHttpClientFactory httpClientFactory, IOptions<WeatherOption> options)
    {
        _httpClientFactory = httpClientFactory;
        _weatherOption = options.Value;
    }

    private string _url(string path) => $"https://api.weatherapi.com/v1/{path}?key={_weatherOption.Key}";

    public async Task<WeatherCurrentRequest?> GetCurrentWeather(string q)
    {
        string url = $"{_url("current.json")}&q={HttpUtility.UrlEncode(q)}";
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

    public async Task<WeatherForecastRequest?> GetForecast(string q, int days)
    {
        string url = $"{_url("forecast.json")}&q={HttpUtility.UrlEncode(q)}&days={days}";
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
