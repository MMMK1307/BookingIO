using BookingIO.Data;
using BookingIO.Models;

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
    }
}
