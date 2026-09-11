using System.ComponentModel.DataAnnotations;

namespace BookingIO.Models;

public class RegisterUserModel
{
    
    [StringLength(255)]
    public required string Name { get; set; }
    [StringLength(255)]
    public required string Login { get; set; }
     [StringLength(255)]
    public required string Email { get; set; }
     [StringLength(255)]
    public required string Password { get; set; }
}
