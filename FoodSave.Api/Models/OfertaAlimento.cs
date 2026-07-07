namespace FoodSave.Api.Models;

public static class EstadoOferta
{
    public const string Disponible = "Disponible";
    public const string Reservada = "Reservada";
    public const string Cerrada = "Cerrada";
    public const string Vencida = "Vencida";
}

public class OfertaAlimento
{
    public int OfertaId { get; set; }
    public int EstablecimientoId { get; set; }
    public Establecimiento? Establecimiento { get; set; }

    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioSugerido { get; set; }
    public string? FotoUrl { get; set; }
    public DateTime DisponibleHasta { get; set; }
    public string Estado { get; set; } = EstadoOferta.Disponible;
    public DateTime FechaPublicacion { get; set; } = DateTime.UtcNow;

    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
