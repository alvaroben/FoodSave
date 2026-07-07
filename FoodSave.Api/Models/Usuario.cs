namespace FoodSave.Api.Models;

public class Usuario
{
    public int UsuarioId { get; set; }
    public int RolId { get; set; }
    public Rol? Rol { get; set; }

    public string NombreCompleto { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public string ContrasenaHash { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string Estado { get; set; } = "Activo";
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public Establecimiento? Establecimiento { get; set; }
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
