using BookingIO.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookingIO.Data;

public class ApplicationDbContext 
    : IdentityDbContext <User, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Space> Spaces { get; set; }
    public DbSet<Reserve> Reserves { get; set; }
    public DbSet<SpaceType> TypeSpaces { get; set; }
}
