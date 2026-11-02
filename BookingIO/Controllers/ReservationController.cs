using BookingIO.Models;
using BookingIO.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookingIO.Controllers;

[Route("Reservation")]
public class ReservationController : Controller
{

    private readonly SpaceServices _spaceServices;
    private readonly ReserveServices _reserveServices;
    private readonly UserManager<User> _userManager;

    public ReservationController(SpaceServices spaceServices, ReserveServices reserveServices, UserManager<User> userManager)
    {
        _spaceServices = spaceServices;
        _reserveServices = reserveServices;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Route("/{controller}/{spaceId}/Reserve")]
    public async Task<IActionResult> Reserve(Guid spaceId)
    {
        var space = await _spaceServices.GetById(spaceId);

        if (space is null) return NotFound();

        SpaceReservationModel model = new(space, await _reserveServices.ListBySpace(space));

        return View("Reserve", model);
    }

    [HttpPost]
    [Route("/{controller}/{spaceId}/Reserve")]
    public async Task<IActionResult> Reserve(Guid spaceId, [FromForm] DateTime start, [FromForm] DateTime end)
    {
        if (start == DateTime.MinValue || end == DateTime.MinValue) return BadRequest();

        var user = await _userManager.GetUserAsync(User);
        var space = await _spaceServices.GetById(spaceId);

        if (user is null || space is null) return NotFound();

        var createResult = await _reserveServices.Create(user, space, start, end);

        if(!createResult) return BadRequest();

        return Ok();
    }
}
