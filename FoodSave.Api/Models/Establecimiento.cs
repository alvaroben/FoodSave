namespace FoodSave.Api.Models;

public class Establecimiento
{
    public int EstablecimientoId { get; set; }
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public string NombreComercial { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string? HorarioAtencion { get; set; }
    public string? TelefonoContacto { get; set; }
    public string? Descripcion { get; set; }
    public string Estado { get; set; } = "Activo";

    public ICollection<OfertaAlimento> Ofertas { get; set; } = new List<OfertaAlimento>();
}
