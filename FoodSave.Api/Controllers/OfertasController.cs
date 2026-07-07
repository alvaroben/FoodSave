using System.Security.Claims;
using FoodSave.Api.Data;
using FoodSave.Api.DTOs;
using FoodSave.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodSave.Api.Controllers;

[ApiController]
[Route("api/ofertas")]
public class OfertasController : ControllerBase
{
    private readonly FoodSaveDbContext _db;

    public OfertasController(FoodSaveDbContext db)
    {
        _db = db;
    }

    private static OfertaResponse ToResponse(OfertaAlimento o) => new(
        o.OfertaId,
        o.EstablecimientoId,
        o.Establecimiento!.NombreComercial,
        o.Titulo,
        o.Descripcion,
        o.Categoria,
        o.Cantidad,
        o.PrecioSugerido,
        o.FotoUrl,
        o.DisponibleHasta,
        o.Estado,
        o.FechaPublicacion
    );

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OfertaResponse>>> Buscar(
        [FromQuery] string? categoria,
        [FromQuery] string? ubicacion,
        [FromQuery] decimal? precioMax,
        [FromQuery] int? minutosRestantes)
    {
        var query = _db.Ofertas
            .Include(o => o.Establecimiento)
            .Where(o => o.Estado == EstadoOferta.Disponible && o.DisponibleHasta > DateTime.UtcNow)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(categoria))
            query = query.Where(o => o.Categoria == categoria);

        if (!string.IsNullOrWhiteSpace(ubicacion))
            query = query.Where(o => o.Establecimiento!.Direccion.Contains(ubicacion));

        if (precioMax.HasValue)
            query = query.Where(o => o.PrecioSugerido <= precioMax.Value);

        if (minutosRestantes.HasValue)
        {
            var limite = DateTime.UtcNow.AddMinutes(minutosRestantes.Value);
            query = query.Where(o => o.DisponibleHasta <= limite);
        }

        var ofertas = await query.OrderBy(o => o.DisponibleHasta).ToListAsync();
        return Ok(ofertas.Select(ToResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OfertaResponse>> ObtenerPorId(int id)
    {
        var oferta = await _db.Ofertas.Include(o => o.Establecimiento).FirstOrDefaultAsync(o => o.OfertaId == id);
        if (oferta is null) return NotFound();
        return Ok(ToResponse(oferta));
    }

    [HttpGet("mias")]
    [Authorize(Roles = RolesSeed.Establecimiento)]
    public async Task<ActionResult<IEnumerable<OfertaResponse>>> MisOfertas()
    {
        var establecimientoId = ObtenerEstablecimientoId();
        var ofertas = await _db.Ofertas
            .Include(o => o.Establecimiento)
            .Where(o => o.EstablecimientoId == establecimientoId)
            .OrderByDescending(o => o.FechaPublicacion)
            .ToListAsync();
        return Ok(ofertas.Select(ToResponse));
    }

    [HttpPost]
    [Authorize(Roles = RolesSeed.Establecimiento)]
    public async Task<ActionResult<OfertaResponse>> Crear(CrearOfertaRequest request)
    {
        var establecimientoId = ObtenerEstablecimientoId();

        var oferta = new OfertaAlimento
        {
            EstablecimientoId = establecimientoId,
            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            Categoria = request.Categoria,
            Cantidad = request.Cantidad,
            PrecioSugerido = request.PrecioSugerido,
            FotoUrl = request.FotoUrl,
            DisponibleHasta = request.DisponibleHasta,
        };

        _db.Ofertas.Add(oferta);
        await _db.SaveChangesAsync();

        await _db.Entry(oferta).Reference(o => o.Establecimiento).LoadAsync();
        return CreatedAtAction(nameof(ObtenerPorId), new { id = oferta.OfertaId }, ToResponse(oferta));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = RolesSeed.Establecimiento)]
    public async Task<ActionResult<OfertaResponse>> Editar(int id, CrearOfertaRequest request)
    {
        var establecimientoId = ObtenerEstablecimientoId();
        var oferta = await _db.Ofertas.Include(o => o.Establecimiento).FirstOrDefaultAsync(o => o.OfertaId == id);

        if (oferta is null) return NotFound();
        if (oferta.EstablecimientoId != establecimientoId) return Forbid();

        oferta.Titulo = request.Titulo;
        oferta.Descripcion = request.Descripcion;
        oferta.Categoria = request.Categoria;
        oferta.Cantidad = request.Cantidad;
        oferta.PrecioSugerido = request.PrecioSugerido;
        oferta.FotoUrl = request.FotoUrl;
        oferta.DisponibleHasta = request.DisponibleHasta;

        await _db.SaveChangesAsync();
        return Ok(ToResponse(oferta));
    }

    private int ObtenerEstablecimientoId()
    {
        var claim = User.FindFirstValue("establecimientoId");
        return int.Parse(claim!);
    }
}
