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

    private async Task CarregarTipos(Guid? selecionado = null)
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
    [Route("/{controller}/Register")]
    public async Task<IActionResult> Register()
    {
        await CarregarTipos();
        return View("CadastrarSpace", new RegisterSpaceModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/{controller}/Register")]
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

       if (!ModelState.IsValid || !await _spaceService.Create(model))
        {
            NotifyError("Erro ao Salvar");
            await CarregarTipos(model.Type);
            return View("CadastrarSpace", model);
        }

        NotifySuccess("Espaço cadastrado com sucesso!");

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] SpaceSearch filtros)
    {
        var spaces = await _spaceService.Search(filtros);

        ViewBag.Results = spaces;

        await CarregarTipos(filtros.Type);

        return View("procurarspace", filtros);
    }

    [HttpGet]
    [Route("/{controller}/{spaceId}")]
    public async Task<IActionResult> Edit([FromRoute] Guid spaceId)
    {
        var model = await _spaceService.GetById(spaceId);

        if (model is null) return NotFound();

        RegisterSpaceModel registerModel = new()
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            Capacity = model.Capacity,
            Localization = model.Localization,
            Type = model.Type.Id,
            Status = model.Status
        };

        await CarregarTipos();

        return View("Edit", registerModel);
    }


    [HttpPost]
    [Route("/{controller}/{spaceId}")]
    public async Task<IActionResult> Edit([FromRoute] Guid spaceId, [FromForm]RegisterSpaceModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Name))
            ModelState.AddModelError(nameof(model.Name), "Informe o nome.");

        if (model.Capacity <= 0)
            ModelState.AddModelError(nameof(model.Capacity),
                "A capacidade deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(model.Localization))
            ModelState.AddModelError(nameof(model.Localization),
                "Informe a localização.");

        var space = await _spaceService.GetById(spaceId);

        if (space is null) return NotFound();

        if (!ModelState.IsValid || !await _spaceService.Edit(space, model))
        {
            NotifyError("Erro ao Salvar");
            await CarregarTipos(model.Type);
            return View("CadastrarSpace", model);
        }

        NotifySuccess("Espaço Salvo com sucesso!");

        return RedirectToAction(nameof(Index));
    }
}
