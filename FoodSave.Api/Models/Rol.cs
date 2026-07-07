namespace FoodSave.Api.Models;

public class Rol
{
    public int RolId { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}

public static class RolesSeed
{
    public const string Establecimiento = "Establecimiento";
    public const string Consumidor = "Consumidor";
}
