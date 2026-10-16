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

public record WeatherCurrentRequest(
    WeatherLocation Location,
    WeatherCurrent Current
);

public record WeatherForecast(
    DateOnly Date,
    float MinTemp,
    float MaxTemp,
    float AvgTemp,
    bool ChanceOfRain,
    WeatherCondition Condition
);
