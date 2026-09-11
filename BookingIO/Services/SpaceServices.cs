using BookingIO.Data;

namespace BookingIO.Services
{
    public class SpaceServices
    {
        private readonly ApplicationDbContext _dbContext;

        public SpaceServices(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
