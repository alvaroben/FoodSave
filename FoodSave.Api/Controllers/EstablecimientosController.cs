using FoodSave.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodSave.Api.Controllers;

[ApiController]
[Route("api/establecimientos")]
public class EstablecimientosController : ControllerBase
{
    private readonly FoodSaveDbContext _db;

    public EstablecimientosController(FoodSaveDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var establecimientos = await _db.Establecimientos
            .Select(e => new
            {
                e.EstablecimientoId,
                e.NombreComercial,
                e.Direccion,
                e.HorarioAtencion,
                e.TelefonoContacto,
                e.Descripcion,
                e.Estado
            })
            .ToListAsync();

        return Ok(establecimientos);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var establecimiento = await _db.Establecimientos.FindAsync(id);
        if (establecimiento is null) return NotFound();
        return Ok(establecimiento);
    }
}
