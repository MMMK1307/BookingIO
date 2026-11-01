namespace BookingIO.Models;

public class RegisterSpaceModel
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int Capacity { get; set; } = 0;
    public string Localization { get; set; } = "";
    public Guid Type { get; set; }
    public SpaceStatus Status { get; set; }
}
