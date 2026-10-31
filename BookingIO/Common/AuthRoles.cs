using BookingIO.Common.ConfigOption;
using BookingIO.Models;
using Microsoft.AspNetCore.Identity;

namespace BookingIO.Common.AuthRoles;

public static class Roles
{
    public const string UserManager = "UserManager";
    public const string ReservationManager = "ReservationManager";

    public static List<string> All => new() { UserManager, ReservationManager };

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

    public static async Task AddAdmin(UserManager<User> userManager, AdminOption adminOption)
    {
        User? admin = await userManager.FindByEmailAsync("admin@adminadmin.com");

        if (admin is null)
        {
            admin = new()
            {
                UserName = "admin",
                Email = "admin@adminadmin.com",
                FullName = "Admin"
            };
            var createResponse = await userManager.CreateAsync(admin, adminOption.Password);
            admin = await userManager.FindByEmailAsync("admin@adminadmin.com");
        }

        await userManager.AddToRolesAsync(admin!, All);
    }
}
