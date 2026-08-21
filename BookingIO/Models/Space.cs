namespace BookingIO.Models;
public class Space
{
    public Space(string name, string description, int capacity)
    {
        Name = name;
        Description = description;
        Capacity = capacity;
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public int Capacity { get; set; }
}

