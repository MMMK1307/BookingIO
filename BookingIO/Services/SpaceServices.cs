using BookingIO.Data;
using BookingIO.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingIO.Services
{
    public class SpaceServices
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly SpaceTypeServices _spaceTypeService;

        public SpaceServices(ApplicationDbContext dbContext, SpaceTypeServices spaceTypeService)
        {
            _dbContext = dbContext;
            _spaceTypeService = spaceTypeService;
        }

        // RegisterSpaceModel model
        public async Task<bool> Create(RegisterSpaceModel model)
        {
            var f = await _spaceTypeService.GetByName(model.Type);

            Space space = new(model.Name, model.Description, model.Capacity, model.Localization, f, model.Status);
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
