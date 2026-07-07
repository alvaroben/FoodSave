namespace FoodSave.Api.Models;

public class ConfirmacionRetiro
{
    public int ConfirmacionId { get; set; }
    public int ReservaId { get; set; }
    public Reserva? Reserva { get; set; }

    public bool ConfirmadoPorConsumidor { get; set; }
    public bool ConfirmadoPorEstablecimiento { get; set; }
    public DateTime? FechaConfirmacion { get; set; }
}
