using Microsoft.AspNetCore.Identity;

namespace BookingIO.Models
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; } = "";
        public DateTime RegisteredAt { get; init; } = DateTime.UtcNow;

        public static User Create(RegisterUserModel registerModel)
        {
            User user = new();
            user.UserName = registerModel.Login;
            user.Email = registerModel.Email;
            user.FullName = registerModel.Name;
            return user;
        }
    }
}
