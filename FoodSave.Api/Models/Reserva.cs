namespace FoodSave.Api.Models;

public static class EstadoReserva
{
    public const string Pendiente = "Pendiente";
    public const string Confirmada = "Confirmada";
    public const string Cancelada = "Cancelada";
}

public class Reserva
{
    public int ReservaId { get; set; }
    public int OfertaId { get; set; }
    public OfertaAlimento? Oferta { get; set; }

    public int ConsumidorId { get; set; }
    public Usuario? Consumidor { get; set; }

    public DateTime FechaReserva { get; set; } = DateTime.UtcNow;
    public DateTime? HoraRetiro { get; set; }
    public string Estado { get; set; } = EstadoReserva.Pendiente;

    public ConfirmacionRetiro? ConfirmacionRetiro { get; set; }
}
