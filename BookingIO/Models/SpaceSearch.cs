namespace BookingIO.Models;

public record SpaceSearch(string? Name, string? Description, int? Capacity, string? Location, Guid? Type, SpaceStatus? Status);
