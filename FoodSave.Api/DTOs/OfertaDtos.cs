namespace FoodSave.Api.DTOs;

public record CrearOfertaRequest(
    string Titulo,
    string? Descripcion,
    string Categoria,
    int Cantidad,
    decimal PrecioSugerido,
    string? FotoUrl,
    DateTime DisponibleHasta
);

public record OfertaResponse(
    int OfertaId,
    int EstablecimientoId,
    string NombreComercial,
    string Titulo,
    string? Descripcion,
    string Categoria,
    int Cantidad,
    decimal PrecioSugerido,
    string? FotoUrl,
    DateTime DisponibleHasta,
    string Estado,
    DateTime FechaPublicacion
);
