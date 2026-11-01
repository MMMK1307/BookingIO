namespace BookingIO.Models.ViewModels;

public class AdminUsersViewModel
{
    public string Search { get; set; } = string.Empty;
    public List<AdminUserItemViewModel> Users { get; set; } = new();
}

public class AdminUserItemViewModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsLocked { get; set; }
}
