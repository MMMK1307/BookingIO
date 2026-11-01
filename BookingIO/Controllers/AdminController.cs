using BookingIO.Common.AuthRoles;
using BookingIO.Data;
using BookingIO.Models;
using BookingIO.Models.ViewModels;
using BookingIO.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingIO.Controllers;

[Authorize(Policy = "RequireAdmin")]
[Route("/Admin")]
public class AdminController : BaseController
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly ReserveServices _reserveServices;

    public AdminController(ApplicationDbContext dbContext, UserManager<User> userManager, ReserveServices reserveServices)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _reserveServices = reserveServices;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var now = DateTime.Now;
        var today = now.Date;
        var tomorrow = today.AddDays(1);

        var totalUsers = await _dbContext.Users.CountAsync();
        var totalSpaces = await _dbContext.Spaces.CountAsync();
        var activeSpaces = await _dbContext.Spaces
            .Where(s => s.Status != SpaceStatus.Blocked)
            .ToListAsync();

        var reservations = await _dbContext.Reserves
            .Include(r => r.Space)
            .Include(r => r.User)
            .Where(r => !string.Equals(r.Status, ReserveStatus.Canceled.ToString(), StringComparison.OrdinalIgnoreCase)
                && !string.Equals(r.Status, "Cancelada", StringComparison.OrdinalIgnoreCase))
            .OrderBy(r => r.Start)
            .ToListAsync();

        var reservationsToday = reservations
            .Count(r => r.Start < tomorrow && r.End >= today);

        var upcomingReservations = reservations
            .Count(r => r.End > now);

        var occupiedSpaceIds = reservations
            .Where(r => r.Start <= now && r.End > now)
            .Select(r => r.Space.Id)
            .Distinct()
            .ToHashSet();

        var occupiedSpaces = activeSpaces.Count(s =>
            occupiedSpaceIds.Contains(s.Id)
            || s.Status == SpaceStatus.Reserved
            || s.Status == SpaceStatus.Destroyed);

        var occupancyRate = activeSpaces.Count == 0
            ? 0
            : (int)Math.Round(occupiedSpaces * 100.0 / activeSpaces.Count);

        var model = new AdminDashboardViewModel
        {
            TotalUsers = totalUsers,
            TotalSpaces = totalSpaces,
            ActiveSpaces = activeSpaces.Count,
            ReservationsToday = reservationsToday,
            UpcomingReservations = upcomingReservations,
            OccupiedSpaces = occupiedSpaces,
            OccupancyRate = occupancyRate
        };

        return View(model);
    }

    [HttpGet("Users")]
    public async Task<IActionResult> Users(string? search = null)
    {
        var query = _dbContext.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim();
            query = query.Where(user =>
                user.FullName.Contains(normalizedSearch)
                || (user.Email != null && user.Email.Contains(normalizedSearch))
                || (user.UserName != null && user.UserName.Contains(normalizedSearch)));
        }

        var users = await query
            .OrderBy(user => user.FullName)
            .ToListAsync();

        var model = new AdminUsersViewModel
        {
            Search = search ?? string.Empty,
            Users = new List<AdminUserItemViewModel>()
        };

        foreach (var user in users)
        {
            var isAdmin = await _userManager.IsInRoleAsync(user, Roles.Admin);
            model.Users.Add(new AdminUserItemViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                RegisteredAt = user.RegisteredAt,
                IsAdmin = isAdmin,
                IsLocked = await _userManager.IsLockedOutAsync(user)
            });
        }

        return View(model);
    }

    [HttpPost("Users/{userId:guid}/ToggleAdmin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAdmin(Guid userId, string? search = null)
    {
        if (userId == Guid.Empty)
            return BadRequest();

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return RedirectToAction("Login", "User");

        var targetUser = await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == userId);
        if (targetUser == null)
            return NotFound();

        if (targetUser.Id == currentUser.Id)
        {
            NotifyError("Ação não permitida", "Você não pode remover a própria administração.");
            return RedirectToAction(nameof(Users), new { search });
        }

        var targetIsAdmin = await IsAdminUser(targetUser);

        if (targetIsAdmin)
        {
            var adminCount = await CountAdminUsersAsync();
            if (adminCount <= 1)
            {
                NotifyError("Ação não permitida", "Não é possível remover o último administrador.");
                return RedirectToAction(nameof(Users), new { search });
            }

            var removeResult = await _userManager.RemoveFromRoleAsync(targetUser, Roles.Admin);
            if (!removeResult.Succeeded)
            {
                NotifyError("Erro", "Não foi possível remover os privilégios do usuário.");
                return RedirectToAction(nameof(Users), new { search });
            }

            NotifySuccess("Privilégios removidos", "O usuário perdeu os acessos administrativos.");
            return RedirectToAction(nameof(Users), new { search });
        }

        var addResult = await _userManager.AddToRoleAsync(targetUser, Roles.Admin);
        if (!addResult.Succeeded)
        {
            NotifyError("Erro", "Não foi possível conceder privilégios ao usuário.");
            return RedirectToAction(nameof(Users), new { search });
        }

        NotifySuccess("Privilégios concedidos", "O usuário agora tem acesso administrativo.");
        return RedirectToAction(nameof(Users), new { search });
    }

    [HttpPost("Users/{userId:guid}/Lock")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LockUser(Guid userId, string? search = null)
    {
        if (userId == Guid.Empty)
            return BadRequest();

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return RedirectToAction("Login", "User");

        var targetUser = await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == userId);
        if (targetUser == null)
            return NotFound();

        if (targetUser.Id == currentUser.Id)
        {
            NotifyError("Ação não permitida", "Você não pode bloquear sua própria conta.");
            return RedirectToAction(nameof(Users), new { search });
        }

        if (await IsAdminUser(targetUser))
        {
            var adminCount = await CountAdminUsersAsync();
            if (adminCount <= 1)
            {
                NotifyError("Ação não permitida", "Não é possível bloquear o último administrador.");
                return RedirectToAction(nameof(Users), new { search });
            }
        }

        var lockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
        var result = await _userManager.SetLockoutEndDateAsync(targetUser, lockoutEnd);
        if (!result.Succeeded)
        {
            NotifyError("Erro", "Não foi possível bloquear o usuário.");
            return RedirectToAction(nameof(Users), new { search });
        }

        NotifySuccess("Usuário bloqueado", "A conta do usuário foi bloqueada com sucesso.");
        return RedirectToAction(nameof(Users), new { search });
    }

    [HttpPost("Users/{userId:guid}/Unlock")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnlockUser(Guid userId, string? search = null)
    {
        if (userId == Guid.Empty)
            return BadRequest();

        var targetUser = await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == userId);
        if (targetUser == null)
            return NotFound();

        var result = await _userManager.SetLockoutEndDateAsync(targetUser, DateTimeOffset.UtcNow.AddMinutes(-1));
        if (!result.Succeeded)
        {
            NotifyError("Erro", "Não foi possível desbloquear o usuário.");
            return RedirectToAction(nameof(Users), new { search });
        }

        NotifySuccess("Usuário desbloqueado", "A conta foi liberada com sucesso.");
        return RedirectToAction(nameof(Users), new { search });
    }

    [HttpGet("Spaces")]
    public async Task<IActionResult> Spaces(string? search = null, Guid? typeId = null, string? localization = null)
    {
        var now = DateTime.Now;
        var query = _dbContext.Spaces
            .Include(space => space.Type)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(space =>
                space.Name.Contains(term)
                || space.Localization.Contains(term)
                || (space.Type != null && space.Type.Name.Contains(term)));
        }

        if (typeId.HasValue)
        {
            query = query.Where(space => space.Type != null && space.Type.Id == typeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(localization))
        {
            var term = localization.Trim();
            query = query.Where(space => space.Localization.Contains(term));
        }

        var spaces = await query
            .OrderBy(space => space.Name)
            .ToListAsync();

        var reservations = await _dbContext.Reserves
            .Include(reserve => reserve.Space)
            .Where(reserve => !string.Equals(reserve.Status, ReserveStatus.Canceled.ToString(), StringComparison.OrdinalIgnoreCase))
            .ToListAsync();

        var model = new AdminSpacesViewModel
        {
            Search = search ?? string.Empty,
            TypeId = typeId,
            Localization = localization ?? string.Empty,
            SpaceTypes = await _dbContext.TypeSpaces.OrderBy(type => type.Name).ToListAsync(),
            Spaces = spaces.Select(space => new AdminSpaceItemViewModel
            {
                Id = space.Id,
                Name = space.Name,
                TypeName = space.Type?.Name ?? "Sem tipo",
                Localization = space.Localization,
                Capacity = space.Capacity,
                Status = space.Status,
                IsOccupiedNow = reservations.Any(reserve =>
                    reserve.Space != null && reserve.Space.Id == space.Id
                    && reserve.Start <= now
                    && reserve.End > now),
                UpcomingReservations = reservations.Count(reserve =>
                    reserve.Space != null && reserve.Space.Id == space.Id && reserve.End > now)
            }).ToList()
        };

        return View(model);
    }

    [HttpPost("Spaces/{id:guid}/Status")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSpaceStatus(Guid id, string status, string? search = null, Guid? typeId = null, string? localization = null)
    {
        if (!Enum.TryParse<SpaceStatus>(status, true, out var parsedStatus))
        {
            NotifyError("Status inválido", "O status do espaço informado não existe.");
            return RedirectToAction(nameof(Spaces), new { search, typeId, localization });
        }

        var space = await _dbContext.Spaces.FirstOrDefaultAsync(item => item.Id == id);
        if (space == null)
            return NotFound();

        space.Status = parsedStatus;
        await _dbContext.SaveChangesAsync();

        NotifySuccess("Status atualizado", $"O espaço '{space.Name}' foi alterado para {GetSpaceStatusLabel(parsedStatus)}.");
        return RedirectToAction(nameof(Spaces), new { search, typeId, localization });
    }

    private static string GetSpaceStatusLabel(SpaceStatus status)
    {
        return status switch
        {
            SpaceStatus.Available => "Disponível",
            SpaceStatus.Blocked => "Bloqueado",
            SpaceStatus.Destroyed => "Em andamento",
            SpaceStatus.Reserved => "Reservado",
            _ => status.ToString()
        };
    }

    [HttpGet("Reservations")]
    public async Task<IActionResult> Reservations(string? userSearch = null, string? spaceSearch = null, string? status = null)
    {
        var query = _dbContext.Reserves
            .Include(reserve => reserve.User)
            .Include(reserve => reserve.Space)
                .ThenInclude(space => space.Type)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(userSearch))
        {
            var term = userSearch.Trim();
            query = query.Where(reserve =>
                reserve.User.FullName.Contains(term)
                || (reserve.User.Email != null && reserve.User.Email.Contains(term))
                || (reserve.User.UserName != null && reserve.User.UserName.Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(spaceSearch))
        {
            var term = spaceSearch.Trim();
            query = query.Where(reserve =>
                reserve.Space.Name.Contains(term)
                || reserve.Space.Localization.Contains(term)
                || (reserve.Space.Type != null && reserve.Space.Type.Name.Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalized = status.Trim();
            query = query.Where(reserve => string.Equals(reserve.Status, normalized, StringComparison.OrdinalIgnoreCase));
        }

        var reservations = await query
            .OrderByDescending(reserve => reserve.Start)
            .ToListAsync();

        var model = new AdminReservationsViewModel
        {
            UserSearch = userSearch ?? string.Empty,
            SpaceSearch = spaceSearch ?? string.Empty,
            Status = status ?? string.Empty,
            Reservations = reservations.Select(reserve => new AdminReservationItemViewModel
            {
                Id = reserve.Id,
                UserName = reserve.User?.FullName ?? "Usuário removido",
                UserEmail = reserve.User?.Email ?? string.Empty,
                SpaceName = reserve.Space?.Name ?? "Espaço removido",
                SpaceType = reserve.Space?.Type?.Name ?? "Sem tipo",
                SpaceLocalization = reserve.Space?.Localization ?? string.Empty,
                Start = reserve.Start,
                End = reserve.End,
                Status = reserve.Status
            }).ToList()
        };

        return View(model);
    }

    [HttpPost("Reservations/{id:guid}/Status")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateReservationStatus(Guid id, string status, string? userSearch = null, string? spaceSearch = null, string? statusFilter = null)
    {
        if (id == Guid.Empty)
            return BadRequest();

        if (!ReserveServices.TryParseStatus(status, out var targetStatus))
        {
            NotifyError("Status inválido", "O status informado não existe no sistema.");
            return RedirectToAction(nameof(Reservations), new { userSearch, spaceSearch, status = statusFilter });
        }

        var reserve = await _dbContext.Reserves
            .Include(r => r.Space)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reserve == null)
            return NotFound();

        if (!ReserveServices.TryParseStatus(reserve.Status, out var currentStatus))
        {
            NotifyError("Status inválido", "O status atual da reserva não pôde ser lido.");
            return RedirectToAction(nameof(Reservations), new { userSearch, spaceSearch, status = statusFilter });
        }

        if (!ReserveServices.IsValidStatusTransition(currentStatus, targetStatus))
        {
            NotifyError("Transição inválida", $"A reserva não pode mudar de '{GetReservationStatusLabel(currentStatus)}' para '{GetReservationStatusLabel(targetStatus)}'.");
            return RedirectToAction(nameof(Reservations), new { userSearch, spaceSearch, status = statusFilter });
        }

        if (targetStatus == ReserveStatus.Reserved)
        {
            var hasConflict = await _reserveServices.HasConflictAsync(reserve.Space.Id, reserve.Start, reserve.End, reserve.Id);
            if (hasConflict)
            {
                NotifyError("Conflito de disponibilidade", "Não foi possível confirmar a reserva porque o espaço já está reservado neste intervalo.");
                return RedirectToAction(nameof(Reservations), new { userSearch, spaceSearch, status = statusFilter });
            }
        }

        reserve.Status = targetStatus.ToString();
        await _dbContext.SaveChangesAsync();

        NotifySuccess("Status atualizado", $"A reserva foi alterada para '{GetReservationStatusLabel(targetStatus)}'.");
        return RedirectToAction(nameof(Reservations), new { userSearch, spaceSearch, status = statusFilter });
    }

    private static string GetReservationStatusLabel(ReserveStatus status)
    {
        return status switch
        {
            ReserveStatus.Released => "Liberada",
            ReserveStatus.Blocked => "Bloqueada",
            ReserveStatus.Canceled => "Cancelada",
            ReserveStatus.Rejected => "Rejeitada",
            ReserveStatus.Reserved => "Reservada",
            _ => status.ToString()
        };
    }

    private async Task<bool> IsAdminUser(User user)
    {
        return await _userManager.IsInRoleAsync(user, Roles.Admin);
    }

    private async Task<int> CountAdminUsersAsync()
    {
        var users = await _dbContext.Users.ToListAsync();
        var count = 0;

        foreach (var user in users)
        {
            if (await IsAdminUser(user))
                count++;
        }

        return count;
    }
}
