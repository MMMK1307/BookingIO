namespace BookingIO.Models;
public class Space
{
    public Space(string name, string description, int capacity, string localization, string tipo, string status)
    {
        Name = name;
        Description = description;
        Capacity = capacity;
        Localization = localization;
        Tipo = tipo;
        Status = status;
       
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public int Capacity { get; set; } 
    public string Localization { get; set; }
    public string Tipo { get; set; }
    public string Status { get; set; }  
}

