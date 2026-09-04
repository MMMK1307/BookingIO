using Microsoft.AspNetCore.Identity;

namespace BookingIO.Models
{
    public class User : IdentityUser<Guid>
    {
        public DateTime RegisteredAt { get; init; } = DateTime.UtcNow;
    }
}
