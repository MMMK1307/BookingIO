using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/space-types")]
public class SpaceTypesController : ControllerBase
{
    //private readonly AppDbContext _context;

    //public SpaceTypesController(AppDbContext context)
    //{
    //    _context = context;
    //}

    //[HttpPost]
    //public async Task<IActionResult> Create(
    //    [FromBody] CreateSpaceTypeRequest request)
    //{
    //    if (string.IsNullOrWhiteSpace(request.Name))
    //    {
    //        return BadRequest("O nome do Space Type é obrigatório.");
    //    }

    //    var exists = await _context.SpaceTypes
    //        .AnyAsync(x => x.Name == request.Name);

    //    if (exists)
    //    {
    //        return Conflict("Esse Space Type já está cadastrado.");
    //    }

    //    var spaceType = new SpaceType(request.Name);

    //    _context.SpaceTypes.Add(spaceType);

    //    await _context.SaveChangesAsync();

    //    return CreatedAtAction(
    //        nameof(GetById),
    //        new { id = spaceType.Id },
    //        spaceType
    //    );
    //}

    //[HttpGet("{id:guid}")]
    //public async Task<IActionResult> GetById(Guid id)
    //{
    //    var spaceType = await _context.SpaceTypes
    //        .FirstOrDefaultAsync(x => x.Id == id);

    //    if (spaceType == null)
    //    {
    //        return NotFound();
    //    }

    //    return Ok(spaceType);
    //}
}
