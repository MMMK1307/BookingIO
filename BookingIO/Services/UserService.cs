using BookingIO.Data;
using BookingIO.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingIO.Services;

public class UserService
{
    private readonly ApplicationDbContext _dbContext;

    public UserService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<User>> GetAll() => await _dbContext.Users.ToListAsync();
    public async Task<User?> GetById(Guid id) => await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<bool> Update(User user, RegisterUserModel editModel)
    {
        user.FullName = editModel.Name;
        user.UserName = editModel.Login;
        user.Email = editModel.Email;
        _dbContext.Users.Update(user);
        int result = await _dbContext.SaveChangesAsync();
        return result >= 1;
    }
}
