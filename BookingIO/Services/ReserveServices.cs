using BookingIO.Data;
using BookingIO.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingIO.Services
{
    public class ReserveServices
    {
        private readonly ApplicationDbContext _dbContext;

        public ReserveServices(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Create(User user, Space space, DateTime start, DateTime end)
        {
            Reserve reserve = new(user, space, "Reservando", start, end);
            _dbContext.Add(reserve);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Reserve>> ListReserveByUser(User user)
        {
            return await _dbContext.Reserves
                .Where(reserve => reserve.User == user)
                .ToListAsync();


        }

        public async Task<Reserve?> GetById(Guid id)
        {
            return await _dbContext.Reserves
                    .Where(space => space.Id == id)
                    .FirstOrDefaultAsync();
        }

        }
}
