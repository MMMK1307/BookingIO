namespace BookingIO.Common.ConfigOption;

public class WeatherOption
{
    public required string Key { get; set; }
    public string Lang { get; set; } = "en";
}

public class AdminOption
{
    public required string Password { get; set; }
}
