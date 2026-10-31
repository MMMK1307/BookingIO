using BookingIO.Data;
using BookingIO.Models;
using BookingIO.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookingIO.Controllers;

public class SpaceController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly SpaceServices _spaceService;

    public SpaceController(ApplicationDbContext context, SpaceServices spaceService)
    {
        _context = context;
        _spaceService = spaceService;
    }

    private async Task CarregarTipos(string? selecionado = null)
    {
        var tipos = await _context.TypeSpaces
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .ToListAsync();

        ViewBag.Types = new SelectList(
            tipos,
            "Id",
            "Name",
            selecionado
        );
    }


    [HttpGet]
    public async Task<IActionResult> Register()
    {
        await CarregarTipos();
        return View("CadastrarSpace", new RegisterSpaceModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterSpaceModel model)
    {

        if (string.IsNullOrWhiteSpace(model.Name))
            ModelState.AddModelError(nameof(model.Name), "Informe o nome.");

        if (model.Capacity <= 0)
            ModelState.AddModelError(nameof(model.Capacity),
                "A capacidade deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(model.Localization))
            ModelState.AddModelError(nameof(model.Localization),
                "Informe a localização.");

        SpaceType? tipo = null;

        if (Guid.TryParse(model.Type, out var tipoId))
            tipo = await _context.TypeSpaces.FindAsync(tipoId);

        if (tipo is null)
            ModelState.AddModelError(nameof(model.Type),
                "Selecione um tipo cadastrado.");

        if (!ModelState.IsValid)
        {
            await CarregarTipos(model.Type);
            return View("CadastrarSpace", model);
        }

        await _spaceService.Create(model);

        NotifySuccess("Espaço cadastrado com sucesso!");

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] SpaceSearch filtros)
    {
        var spaces = await _spaceService.Search(filtros);

        ViewBag.Results = spaces;

        await CarregarTipos(filtros.Type);

        return View("procurarspace", filtros);
    }
}
