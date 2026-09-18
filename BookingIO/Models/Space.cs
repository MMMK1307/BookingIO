namespace BookingIO.Models;
public class Space
{
    public Space(string name, string description, int capacity, string localization, SpaceType type, string status)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Capacity = capacity;
        Localization = localization;
        Type = type;
        Status = status;
    }

    private Space() { }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Capacity { get; set; } 
    public string Localization { get; set; }
    public SpaceType Type { get; set; }
    public string Status { get; set; }
}

