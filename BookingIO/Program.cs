using BookingIO.Common.ConfigOption;
using BookingIO.Data;
using BookingIO.Models;
using BookingIO.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// AUTH: https://dev.to/cristiansifuentes/aspnet-core-identity-in-net-10-from-login-page-to-production-grade-security-4n6o

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection is null");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySQL(connectionString);
});

builder.Services.AddScoped<SpaceTypeServices>();
builder.Services.AddScoped<SpaceServices>();
builder.Services.AddScoped<ReserveServices>();
builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddHttpClient();

builder.Services.AddDefaultIdentity<User>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = true;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddRoles<IdentityRole<Guid>>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".IdentityNet7.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;

    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";

    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy =>
        policy.RequireRole("Admin"));
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<WeatherOption>(builder.Configuration.GetSection("WeatherAPI"));
builder.Services.Configure<AdminOption>(builder.Configuration.GetSection("Admin"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    await BookingIO.Common.AuthRoles.Roles.AddDefaultRoles(
        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>()
    );

    await BookingIO.Common.AuthRoles.Roles.AddAdmin(
        scope.ServiceProvider.GetRequiredService<UserManager<User>>(),
        scope.ServiceProvider.GetRequiredService<IOptions<AdminOption>>().Value
    );
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
