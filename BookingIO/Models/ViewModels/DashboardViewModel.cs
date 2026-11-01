namespace BookingIO.Models.ViewModels;

public class DashboardViewModel
{
    public string UserName { get; set; } = "";
    public int FutureReservations { get; set; }
    public int TodayReservations { get; set; }
    public int AvailableSpaces { get; set; }
    public int TotalSpaces { get; set; }
    public int OccupiedSpaces { get; set; }
    public int OccupancyRate { get; set; }
    public List<Reserve> NextReservations { get; set; } = new();
    public List<DashboardDayItem> NextSevenDays { get; set; } = new();
}

public class DashboardDayItem
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
}
