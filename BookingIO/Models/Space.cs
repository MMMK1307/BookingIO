namespace BookingIO.Models;

public enum SpaceStatus
{
    Available,
    Blocked,
    InProgress,
    Reserved,

}

public class Space
{
    public Space(string name, string description, int capacity, string localization, SpaceType type, SpaceStatus status)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Capacity = capacity;
        Localization = localization;
        Type = type;
        Status = status;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Space() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Capacity { get; set; } 
    public string Localization { get; set; }
    public SpaceType Type { get; set; }
    public SpaceStatus Status { get; set; }
}

