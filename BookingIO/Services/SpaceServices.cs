using BookingIO.Data;
using BookingIO.Models;

namespace BookingIO.Services
{
    public class SpaceServices
    {
        private readonly ApplicationDbContext _dbContext;

        public SpaceServices(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // RegisterSpaceModel model
        public async Task<bool> Create(RegisterSpaceModel model)
        {
            Space space = new(model.Name, model.Description, model.Capacity, model.Localization, null, model.Status);
            _dbContext.Add(space);
            await _dbContext.SaveChangesAsync();
            return true;
        }


    }
}
