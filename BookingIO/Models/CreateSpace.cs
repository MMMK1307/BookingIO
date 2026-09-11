namespace BookingIO.Models;

public class CreateSpace
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int Capacity { get; set; } = 0;
    public string Localization { get; set; } = "";
    public string Tipo { get; set; } = "";
    public string Status { get; set; } = "";
}
