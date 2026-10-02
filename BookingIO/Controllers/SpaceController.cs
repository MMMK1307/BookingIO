using BookingIO.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookingIO.Controllers
{
    public class SpaceController : Controller
    {
        public IActionResult Index()
        {
            var model = new RegisterSpaceModel();
            return View("CadastrarSpace", model);
        }
        public async Task<IActionResult> Search()
        {
            var model = new SpaceSearch(null, null, null, null, null, null);
            return View("ProcurarSpace", model);
        }
    }
}

