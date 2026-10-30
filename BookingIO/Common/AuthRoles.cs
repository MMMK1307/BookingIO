using Microsoft.AspNetCore.Identity;

namespace BookingIO.Common.AuthRoles;

public static class Roles
{
    public const string UserManager = "UserManager";
    public const string ReservationManager = "ReservationManager";

    public static async Task AddDefaultRoles(RoleManager<IdentityRole<Guid>> roleManager)
    {
        string[] defaultRoles = { UserManager, ReservationManager };
        foreach(var role in defaultRoles)
        {
            if (await roleManager.RoleExistsAsync(role)) continue;
            var identityRole = new IdentityRole<Guid>(role) { Id = Guid.NewGuid() };
            await roleManager.CreateAsync(identityRole);
        }
    }
}
