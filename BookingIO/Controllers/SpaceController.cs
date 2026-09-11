using BookingIO.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookingIO.Controllers
{
    public class SpaceController : Controller
    {
        public IActionResult Index()
        {
            var model = new CreateSpace();
            return View("CadastrarSpace", model);
        }
    }
}
