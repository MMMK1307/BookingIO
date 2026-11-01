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
            if (user == null || space == null)
                return false;

            if (end <= start)
                return false;

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var lockedSpace = await _dbContext.Spaces
                    .FromSqlRaw("SELECT * FROM Spaces WHERE Id = {0} FOR UPDATE", space.Id)
                    .SingleOrDefaultAsync();

                if (lockedSpace == null)
                    return false;

                if (lockedSpace.Status == SpaceStatus.Blocked)
                    return false;

                if (lockedSpace.Status == SpaceStatus.Reserved || lockedSpace.Status == SpaceStatus.Destroyed)
                    return false;

                if (await HasConflictAsync(lockedSpace.Id, start, end))
                    return false;

                var reserve = new Reserve(user, lockedSpace, ReserveStatus.Reserved, start, end);
                _dbContext.Add(reserve);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<List<Reserve>> ListReserveByUser(User user)
        {
            if (user == null)
                return new List<Reserve>();

            return await _dbContext.Reserves
                .Include(reserve => reserve.Space)
                .Include(reserve => reserve.User)
                .Where(reserve => reserve.User != null && reserve.User.Id == user.Id)
                .ToListAsync();
        }

        public async Task<Reserve?> GetById(Guid id)
        {
            return await _dbContext.Reserves
                .Include(reserve => reserve.Space)
                .Include(reserve => reserve.User)
                .FirstOrDefaultAsync(reserve => reserve.Id == id);
        }

        public async Task<bool> CheckReservation(ReserveRequest request)
        {
            if (request == null)
                return false;

            if (request.End <= request.Start)
                return false;

            return !await HasConflictAsync(request.Space.Id, request.Start, request.End);
        }

        public static bool TryParseStatus(string? status, out ReserveStatus parsedStatus)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                parsedStatus = default;
                return false;
            }

            var normalized = status.Trim();

            if (Enum.TryParse<ReserveStatus>(normalized, true, out var parsed))
            {
                parsedStatus = parsed;
                return true;
            }

            switch (normalized.ToLowerInvariant())
            {
                case "cancelada":
                case "cancelado":
                    parsedStatus = ReserveStatus.Canceled;
                    return true;
                case "bloqueada":
                case "bloqueado":
                    parsedStatus = ReserveStatus.Blocked;
                    return true;
                case "liberada":
                case "liberado":
                    parsedStatus = ReserveStatus.Released;
                    return true;
                case "rejeitada":
                case "rejeitado":
                    parsedStatus = ReserveStatus.Rejected;
                    return true;
                case "reservada":
                case "reservado":
                    parsedStatus = ReserveStatus.Reserved;
                    return true;
                default:
                    parsedStatus = default;
                    return false;
            }
        }

        public static bool IsValidStatusTransition(ReserveStatus currentStatus, ReserveStatus targetStatus)
        {
            if (currentStatus == targetStatus)
                return true;

            return (currentStatus, targetStatus) switch
            {
                (ReserveStatus.Reserved, ReserveStatus.Canceled) => true,
                (ReserveStatus.Reserved, ReserveStatus.Rejected) => true,
                (ReserveStatus.Reserved, ReserveStatus.Blocked) => true,
                (ReserveStatus.Reserved, ReserveStatus.Released) => true,
                (ReserveStatus.Blocked, ReserveStatus.Canceled) => true,
                (ReserveStatus.Blocked, ReserveStatus.Rejected) => true,
                (ReserveStatus.Blocked, ReserveStatus.Released) => true,
                (ReserveStatus.Blocked, ReserveStatus.Reserved) => true,
                (ReserveStatus.Released, ReserveStatus.Canceled) => true,
                (ReserveStatus.Released, ReserveStatus.Rejected) => true,
                (ReserveStatus.Released, ReserveStatus.Reserved) => true,
                _ => false
            };
        }

        public async Task<bool> HasConflictAsync(Guid spaceId, DateTime start, DateTime end, Guid? excludeReserveId = null)
        {
            if (end <= start)
                return true;

            return await _dbContext.Reserves
                .Where(reserve => reserve.Space != null && reserve.Space.Id == spaceId)
                .Where(reserve => !excludeReserveId.HasValue || reserve.Id != excludeReserveId.Value)
                .Where(reserve => !string.Equals(reserve.Status, ReserveStatus.Canceled.ToString(), StringComparison.OrdinalIgnoreCase))
                .Where(reserve => !string.Equals(reserve.Status, "Cancelada", StringComparison.OrdinalIgnoreCase))
                .Where(reserve => !string.Equals(reserve.Status, ReserveStatus.Rejected.ToString(), StringComparison.OrdinalIgnoreCase))
                .Where(reserve => !string.Equals(reserve.Status, "Rejeitada", StringComparison.OrdinalIgnoreCase))
                .Where(reserve => reserve.Start < end && reserve.End > start)
                .AnyAsync();
        }

        public async Task<bool> CanTransitionStatusAsync(Guid reserveId, ReserveStatus targetStatus)
        {
            var reserve = await GetById(reserveId);
            if (reserve == null)
                return false;

            if (!TryParseStatus(reserve.Status, out var currentStatus))
                return false;

            if (!IsValidStatusTransition(currentStatus, targetStatus))
                return false;

            if (targetStatus == ReserveStatus.Reserved)
                return !await HasConflictAsync(reserve.Space.Id, reserve.Start, reserve.End, reserve.Id);

            return true;
        }

        public async Task<bool> CanCreateReservationAsync(Space space, DateTime start, DateTime end)
        {
            if (space == null)
                return false;

            if (space.Status == SpaceStatus.Blocked)
                return false;

            if (space.Status == SpaceStatus.Reserved || space.Status == SpaceStatus.Destroyed)
                return false;

            if (start >= end)
                return false;

            return !await HasConflictAsync(space.Id, start, end);
        }
    }
}
