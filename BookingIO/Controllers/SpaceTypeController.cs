
using BookingIO.Models;
using BookingIO.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class SpaceTypeController : Controller
{
    private readonly SpaceTypeServices _spaceTypeService;

    public SpaceTypeController(SpaceTypeServices spaceTypeService)
    {
        _spaceTypeService = spaceTypeService;
    }

    public IActionResult Create()
    {
        return View("CadastrarSpaceType");
    }
}
