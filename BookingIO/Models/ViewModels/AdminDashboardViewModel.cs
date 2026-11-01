namespace BookingIO.Models.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalSpaces { get; set; }
    public int ActiveSpaces { get; set; }
    public int ReservationsToday { get; set; }
    public int UpcomingReservations { get; set; }
    public int OccupiedSpaces { get; set; }
    public int OccupancyRate { get; set; }
}
