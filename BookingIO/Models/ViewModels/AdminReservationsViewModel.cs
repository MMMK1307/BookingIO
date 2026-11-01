namespace BookingIO.Models.ViewModels;

public class AdminReservationsViewModel
{
    public string UserSearch { get; set; } = string.Empty;
    public string SpaceSearch { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<AdminReservationItemViewModel> Reservations { get; set; } = new();
}

public class AdminReservationItemViewModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string SpaceName { get; set; } = string.Empty;
    public string SpaceType { get; set; } = string.Empty;
    public string SpaceLocalization { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Status { get; set; } = string.Empty;
}
