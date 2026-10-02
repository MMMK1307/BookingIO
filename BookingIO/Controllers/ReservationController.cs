using Microsoft.AspNetCore.Mvc;

namespace BookingIO.Controllers;

public class ReservationController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
