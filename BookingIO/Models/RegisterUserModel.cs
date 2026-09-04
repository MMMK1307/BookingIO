namespace BookingIO.Models;

public class RegisterUserModel
{
    public required string Name { get; set; }
    public required string Login { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
