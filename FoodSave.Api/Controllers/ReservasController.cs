using System.Security.Claims;
using FoodSave.Api.Data;
using FoodSave.Api.DTOs;
using FoodSave.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodSave.Api.Controllers;

[ApiController]
[Route("api/reservas")]
[Authorize]
public class ReservasController : ControllerBase
{
    private readonly FoodSaveDbContext _db;

    public ReservasController(FoodSaveDbContext db)
    {
        _db = db;
    }

    private static ReservaResponse ToResponse(Reserva r) => new(
        r.ReservaId,
        r.OfertaId,
        r.Oferta!.Titulo,
        r.Oferta.Establecimiento!.NombreComercial,
        r.ConsumidorId,
        r.Consumidor!.NombreCompleto,
        r.FechaReserva,
        r.HoraRetiro,
        r.Estado,
        r.ConfirmacionRetiro?.ConfirmadoPorConsumidor ?? false,
        r.ConfirmacionRetiro?.ConfirmadoPorEstablecimiento ?? false
    );

    private int ObtenerUsuarioId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private int? ObtenerEstablecimientoId()
    {
        var claim = User.FindFirstValue("establecimientoId");
        return claim is null ? null : int.Parse(claim);
    }

    [HttpPost]
    [Authorize(Roles = RolesSeed.Consumidor)]
    public async Task<ActionResult<ReservaResponse>> Reservar(CrearReservaRequest request)
    {
        var oferta = await _db.Ofertas
            .Include(o => o.Establecimiento)
            .FirstOrDefaultAsync(o => o.OfertaId == request.OfertaId);

        if (oferta is null) return NotFound("Oferta no encontrada.");
        if (oferta.Estado != EstadoOferta.Disponible || oferta.DisponibleHasta <= DateTime.UtcNow)
            return BadRequest("Esta oferta ya no está disponible.");

        var consumidorId = ObtenerUsuarioId();

        var reserva = new Reserva
        {
            OfertaId = oferta.OfertaId,
            ConsumidorId = consumidorId,
            Estado = EstadoReserva.Pendiente,
        };
        _db.Reservas.Add(reserva);

        oferta.Estado = EstadoOferta.Reservada;

        await _db.SaveChangesAsync();

        _db.ConfirmacionesRetiro.Add(new ConfirmacionRetiro { ReservaId = reserva.ReservaId });
        await _db.SaveChangesAsync();

        var creada = await _db.Reservas
            .Include(r => r.Oferta).ThenInclude(o => o!.Establecimiento)
            .Include(r => r.Consumidor)
            .Include(r => r.ConfirmacionRetiro)
            .FirstAsync(r => r.ReservaId == reserva.ReservaId);

        return CreatedAtAction(nameof(ObtenerPorId), new { id = reserva.ReservaId }, ToResponse(creada));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReservaResponse>> ObtenerPorId(int id)
    {
        var reserva = await CargarReserva(id);
        if (reserva is null) return NotFound();
        return Ok(ToResponse(reserva));
    }

    [HttpGet("mias")]
    [Authorize(Roles = RolesSeed.Consumidor)]
    public async Task<ActionResult<IEnumerable<ReservaResponse>>> MisReservas()
    {
        var consumidorId = ObtenerUsuarioId();
        var reservas = await Consulta()
            .Where(r => r.ConsumidorId == consumidorId)
            .OrderByDescending(r => r.FechaReserva)
            .ToListAsync();
        return Ok(reservas.Select(ToResponse));
    }

    [HttpGet("establecimiento")]
    [Authorize(Roles = RolesSeed.Establecimiento)]
    public async Task<ActionResult<IEnumerable<ReservaResponse>>> ReservasDeEstablecimiento()
    {
        var establecimientoId = ObtenerEstablecimientoId();
        var reservas = await Consulta()
            .Where(r => r.Oferta!.EstablecimientoId == establecimientoId)
            .OrderByDescending(r => r.FechaReserva)
            .ToListAsync();
        return Ok(reservas.Select(ToResponse));
    }

    [HttpPost("{id:int}/confirmar-consumidor")]
    [Authorize(Roles = RolesSeed.Consumidor)]
    public async Task<ActionResult<ReservaResponse>> ConfirmarConsumidor(int id)
    {
        var reserva = await CargarReserva(id);
        if (reserva is null) return NotFound();
        if (reserva.ConsumidorId != ObtenerUsuarioId()) return Forbid();

        reserva.ConfirmacionRetiro!.ConfirmadoPorConsumidor = true;
        await FinalizarSiCorresponde(reserva);
        await _db.SaveChangesAsync();
        return Ok(ToResponse(reserva));
    }

    [HttpPost("{id:int}/confirmar-establecimiento")]
    [Authorize(Roles = RolesSeed.Establecimiento)]
    public async Task<ActionResult<ReservaResponse>> ConfirmarEstablecimiento(int id)
    {
        var reserva = await CargarReserva(id);
        if (reserva is null) return NotFound();
        if (reserva.Oferta!.EstablecimientoId != ObtenerEstablecimientoId()) return Forbid();

        reserva.ConfirmacionRetiro!.ConfirmadoPorEstablecimiento = true;
        await FinalizarSiCorresponde(reserva);
        await _db.SaveChangesAsync();
        return Ok(ToResponse(reserva));
    }

    [HttpPost("{id:int}/cancelar")]
    public async Task<ActionResult<ReservaResponse>> Cancelar(int id)
    {
        var reserva = await CargarReserva(id);
        if (reserva is null) return NotFound();

        var esConsumidorDueno = reserva.ConsumidorId == ObtenerUsuarioId();
        var esEstablecimientoDueno = reserva.Oferta!.EstablecimientoId == ObtenerEstablecimientoId();
        if (!esConsumidorDueno && !esEstablecimientoDueno) return Forbid();

        reserva.Estado = EstadoReserva.Cancelada;
        reserva.Oferta.Estado = EstadoOferta.Disponible;

        await _db.SaveChangesAsync();
        return Ok(ToResponse(reserva));
    }

    private async Task FinalizarSiCorresponde(Reserva reserva)
    {
        if (reserva.ConfirmacionRetiro!.ConfirmadoPorConsumidor && reserva.ConfirmacionRetiro.ConfirmadoPorEstablecimiento)
        {
            reserva.ConfirmacionRetiro.FechaConfirmacion = DateTime.UtcNow;
            reserva.Estado = EstadoReserva.Confirmada;
            reserva.HoraRetiro = DateTime.UtcNow;
            reserva.Oferta!.Estado = EstadoOferta.Cerrada;
        }
        await Task.CompletedTask;
    }

    private IQueryable<Reserva> Consulta() => _db.Reservas
        .Include(r => r.Oferta).ThenInclude(o => o!.Establecimiento)
        .Include(r => r.Consumidor)
        .Include(r => r.ConfirmacionRetiro);

    private Task<Reserva?> CargarReserva(int id) => Consulta().FirstOrDefaultAsync(r => r.ReservaId == id);
}
