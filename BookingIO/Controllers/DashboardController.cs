using BookingIO.Data;
using BookingIO.Models;
using BookingIO.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingIO.Controllers;

[Authorize]
public class DashboardController : BaseController
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<User> _userManager;

    public DashboardController(ApplicationDbContext dbContext, UserManager<User> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return RedirectToAction("Login", "User");

        var now = DateTime.Now;
        var today = now.Date;
        var tomorrow = today.AddDays(1);

        var reservations = await _dbContext.Reserves
            .Include(r => r.Space)
            .Where(r => r.User.Id == user.Id)
            .OrderBy(r => r.Start)
            .ToListAsync();

        var spaces = await _dbContext.Spaces
            .Include(s => s.Type)
            .ToListAsync();

        var activeReservations = reservations
            .Where(r => r.Start <= now && r.End > now && !IsCanceled(r))
            .ToList();

        var activeSpaces = spaces
            .Where(s => s.Status != SpaceStatus.Blocked)
            .ToList();

        var occupiedSpaceIds = activeReservations
            .Select(r => r.Space.Id)
            .Distinct()
            .ToHashSet();

        var occupiedSpaces = activeSpaces.Count(s =>
            occupiedSpaceIds.Contains(s.Id) ||
            s.Status == SpaceStatus.Reserved ||
            s.Status == SpaceStatus.InProgress);

        var totalSpaces = activeSpaces.Count;
        var availableSpaces = Math.Max(0, totalSpaces - occupiedSpaces);
        var occupancyRate = totalSpaces == 0 ? 0 : (int)Math.Round(occupiedSpaces * 100.0 / totalSpaces);

        var futureReservations = reservations
            .Where(r => r.End > now && !IsCanceled(r))
            .ToList();

        var model = new DashboardViewModel
        {
            UserName = string.IsNullOrWhiteSpace(user.FullName) ? user.UserName ?? "Usuário" : user.FullName,
            FutureReservations = futureReservations.Count,
            TodayReservations = reservations.Count(r => r.Start < tomorrow && r.End >= today && !IsCanceled(r)),
            AvailableSpaces = availableSpaces,
            TotalSpaces = totalSpaces,
            OccupiedSpaces = occupiedSpaces,
            OccupancyRate = occupancyRate,
            NextReservations = futureReservations.Take(5).ToList(),
            NextSevenDays = Enumerable.Range(0, 7)
                .Select(offset =>
                {
                    var date = today.AddDays(offset);
                    var endOfDay = date.AddDays(1);

                    return new DashboardDayItem
                    {
                        Date = date,
                        Count = reservations.Count(r =>
                            r.Start < endOfDay &&
                            r.End >= date &&
                            !IsCanceled(r))
                    };
                })
                .ToList()
        };

        return View(model);
    }

    private static bool IsCanceled(Reserve reserve)
    {
        return string.Equals(reserve.Status, ReserveStatus.Canceled.ToString(), StringComparison.OrdinalIgnoreCase)
            || string.Equals(reserve.Status, "Cancelada", StringComparison.OrdinalIgnoreCase);
    }
}
