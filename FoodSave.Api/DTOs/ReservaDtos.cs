namespace FoodSave.Api.DTOs;

public record CrearReservaRequest(int OfertaId);

public record ReservaResponse(
    int ReservaId,
    int OfertaId,
    string OfertaTitulo,
    string NombreComercial,
    int ConsumidorId,
    string ConsumidorNombre,
    DateTime FechaReserva,
    DateTime? HoraRetiro,
    string Estado,
    bool ConfirmadoPorConsumidor,
    bool ConfirmadoPorEstablecimiento
);
