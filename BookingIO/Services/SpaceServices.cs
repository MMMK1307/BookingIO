using BookingIO.Data;
using BookingIO.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<Space>> Search(SpaceSearch search)
        {
            var d = _dbContext.Spaces.AsQueryable<Space>();

            if (!string.IsNullOrEmpty(search.Name))
                d = d.Where(s => s.Name.Contains(search.Name));

            if (!string.IsNullOrEmpty(search.Description))
                d = d.Where(s => s.Description.Contains(search.Description));

            if (search.Capacity is not null)
                d = d.Where(s => s.Capacity == search.Capacity);

            if (!string.IsNullOrEmpty(search.Location))
                d = d.Where(s => s.Localization.Contains(search.Location));

            if (!string.IsNullOrEmpty(search.Type))
                d = d.Where(s => s.Type.Name == search.Type);

            return await d.ToListAsync();
        }
    }
}
