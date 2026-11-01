namespace BookingIO.Models.ViewModels;

public class AdminSpacesViewModel
{
    public string Search { get; set; } = string.Empty;
    public Guid? TypeId { get; set; }
    public string Localization { get; set; } = string.Empty;
    public List<SpaceType> SpaceTypes { get; set; } = new();
    public List<AdminSpaceItemViewModel> Spaces { get; set; } = new();
}

public class AdminSpaceItemViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string Localization { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public SpaceStatus Status { get; set; }
    public bool IsOccupiedNow { get; set; }
    public int UpcomingReservations { get; set; }
}
