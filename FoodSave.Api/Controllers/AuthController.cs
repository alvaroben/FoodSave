using FoodSave.Api.Data;
using FoodSave.Api.DTOs;
using FoodSave.Api.Models;
using FoodSave.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodSave.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly FoodSaveDbContext _db;
    private readonly TokenService _tokenService;

    public AuthController(FoodSaveDbContext db, TokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    [HttpPost("registro/consumidor")]
    public async Task<ActionResult<AuthResponse>> RegistrarConsumidor(RegistroConsumidorRequest request)
    {
        if (await _db.Usuarios.AnyAsync(u => u.CorreoElectronico == request.CorreoElectronico))
        {
            return Conflict("Ya existe un usuario con ese correo electrónico.");
        }

        var rol = await _db.Roles.FirstAsync(r => r.Nombre == RolesSeed.Consumidor);

        var usuario = new Usuario
        {
            RolId = rol.RolId,
            NombreCompleto = request.NombreCompleto,
            CorreoElectronico = request.CorreoElectronico,
            ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(request.Contrasena),
            Telefono = request.Telefono,
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        var token = _tokenService.GenerarToken(usuario, rol.Nombre, null);
        return Ok(new AuthResponse(token, usuario.UsuarioId, usuario.NombreCompleto, rol.Nombre, null));
    }

    [HttpPost("registro/establecimiento")]
    public async Task<ActionResult<AuthResponse>> RegistrarEstablecimiento(RegistroEstablecimientoRequest request)
    {
        if (await _db.Usuarios.AnyAsync(u => u.CorreoElectronico == request.CorreoElectronico))
        {
            return Conflict("Ya existe un usuario con ese correo electrónico.");
        }

        var rol = await _db.Roles.FirstAsync(r => r.Nombre == RolesSeed.Establecimiento);

        var usuario = new Usuario
        {
            RolId = rol.RolId,
            NombreCompleto = request.NombreCompleto,
            CorreoElectronico = request.CorreoElectronico,
            ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(request.Contrasena),
            Telefono = request.Telefono,
        };
        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        var establecimiento = new Establecimiento
        {
            UsuarioId = usuario.UsuarioId,
            NombreComercial = request.NombreComercial,
            Direccion = request.Direccion,
            HorarioAtencion = request.HorarioAtencion,
            TelefonoContacto = request.TelefonoContacto,
            Descripcion = request.Descripcion,
        };
        _db.Establecimientos.Add(establecimiento);
        await _db.SaveChangesAsync();

        var token = _tokenService.GenerarToken(usuario, rol.Nombre, establecimiento.EstablecimientoId);
        return Ok(new AuthResponse(token, usuario.UsuarioId, usuario.NombreCompleto, rol.Nombre, establecimiento.EstablecimientoId));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var usuario = await _db.Usuarios
            .Include(u => u.Rol)
            .Include(u => u.Establecimiento)
            .FirstOrDefaultAsync(u => u.CorreoElectronico == request.CorreoElectronico);

        if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.ContrasenaHash))
        {
            return Unauthorized("Correo o contraseña incorrectos.");
        }

        var token = _tokenService.GenerarToken(usuario, usuario.Rol!.Nombre, usuario.Establecimiento?.EstablecimientoId);
        return Ok(new AuthResponse(token, usuario.UsuarioId, usuario.NombreCompleto, usuario.Rol.Nombre, usuario.Establecimiento?.EstablecimientoId));
    }
}
