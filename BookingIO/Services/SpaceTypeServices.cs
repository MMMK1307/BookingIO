using BookingIO.Data;
using BookingIO.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingIO.Services;

public class SpaceTypeServices
{
    private readonly ApplicationDbContext _dbContext;

    public SpaceTypeServices(ApplicationDbContext db_context)
    {
        _dbContext = db_context;
    }

    public async Task<bool> Create(string name)
    {
        SpaceType spaceType = new(name);
        _dbContext.Add(spaceType);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<SpaceType>> ListAll()
    {
        return await _dbContext.TypeSpaces.ToListAsync();
    }

    public async Task<SpaceType?> GetById(Guid id)
    {
        return await _dbContext.TypeSpaces
                .Where(space => space.Id == id)
                .FirstOrDefaultAsync();
    }
}
