using BookingIO.Data;
using System.Text.Json.Serialization;

namespace BookingIO.Models;

public record WeatherLocation(
    string Name,
    string Region,
    string Country,
    float Lat,
    float Log,
    string TimeZoneId
);

public record WeatherCondition(
    string Text,
    string Icon,
    int Code
);

public record WeatherCurrent(
    [property: JsonPropertyName("last_updated")]
    [property: JsonConverter(typeof(DateTimeConverter))]
    DateTime Date,
    [property: JsonPropertyName("temp_c")]
    float Temp,
    [property: JsonPropertyName("is_day")]
    [property: JsonConverter(typeof(IntBoolConverter))]
    bool IsDay,
    WeatherCondition Condition
);

public record WeatherForecastReques(
    WeatherLocation Location,
    WeatherCurrent Current
);

public record WeatherForecastDayData(
    [property: JsonPropertyName("maxtemp_c")]
    float MaxTemp,
    [property: JsonPropertyName("mintemp_c")]
    float MinTemp,
    [property: JsonPropertyName("avgtemp_c")]
    float AvgTemp,
    WeatherCondition Condition,
    int DailyChanceOfRain,
    [property: JsonPropertyName("maxwind_kph")]
    float WindSpeed
);

public record WeatherForecastDay(
    DateOnly Date,
    [property: JsonPropertyName("day")]
    WeatherForecastDayData Data
);

public record WeatherForecast(
    [property: JsonPropertyName("forecasday")]
    List<WeatherForecastDay> Days
);

public record WeatherForecastRequest(
    WeatherLocation Location,
    WeatherForecast Forcast   
);
