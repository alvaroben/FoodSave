namespace FoodSave.Api.DTOs;

public record RegistroConsumidorRequest(
    string NombreCompleto,
    string CorreoElectronico,
    string Contrasena,
    string? Telefono
);

public record RegistroEstablecimientoRequest(
    string NombreCompleto,
    string CorreoElectronico,
    string Contrasena,
    string? Telefono,
    string NombreComercial,
    string Direccion,
    string? HorarioAtencion,
    string? TelefonoContacto,
    string? Descripcion
);

public record LoginRequest(string CorreoElectronico, string Contrasena);

public record AuthResponse(string Token, int UsuarioId, string NombreCompleto, string Rol, int? EstablecimientoId);
