using Microsoft.AspNetCore.Mvc;

namespace BookingIO.Controllers
{
    public class SpaceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
