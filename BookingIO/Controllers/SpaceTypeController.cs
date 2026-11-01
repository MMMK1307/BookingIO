using BookingIO.Services;
using Microsoft.AspNetCore.Mvc;

public class SpaceTypeController : Controller
{
    private readonly SpaceTypeServices _spaceTypeService;

    public SpaceTypeController(SpaceTypeServices spaceTypeService)
    {
        _spaceTypeService = spaceTypeService;
    }

    public IActionResult Create()
    {
        return View("Create");
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] string name)
    {
        var result = await _spaceTypeService.Create(name);

        return RedirectToAction("Index", "Space");
    }
}
