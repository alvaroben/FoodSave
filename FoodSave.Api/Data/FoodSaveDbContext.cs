using FoodSave.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodSave.Api.Data;

public class FoodSaveDbContext : DbContext
{
    public FoodSaveDbContext(DbContextOptions<FoodSaveDbContext> options) : base(options)
    {
    }

    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Establecimiento> Establecimientos => Set<Establecimiento>();
    public DbSet<OfertaAlimento> Ofertas => Set<OfertaAlimento>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<ConfirmacionRetiro> ConfirmacionesRetiro => Set<ConfirmacionRetiro>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasIndex(r => r.Nombre).IsUnique();
            entity.HasData(
                new Rol { RolId = 1, Nombre = RolesSeed.Establecimiento },
                new Rol { RolId = 2, Nombre = RolesSeed.Consumidor }
            );
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasIndex(u => u.CorreoElectronico).IsUnique();
            entity.HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Establecimiento>(entity =>
        {
            entity.HasOne(e => e.Usuario)
                .WithOne(u => u.Establecimiento)
                .HasForeignKey<Establecimiento>(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OfertaAlimento>(entity =>
        {
            entity.HasKey(o => o.OfertaId);
            entity.Property(o => o.PrecioSugerido).HasColumnType("decimal(10,2)");
            entity.HasOne(o => o.Establecimiento)
                .WithMany(e => e.Ofertas)
                .HasForeignKey(o => o.EstablecimientoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasOne(r => r.Oferta)
                .WithMany(o => o.Reservas)
                .HasForeignKey(r => r.OfertaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Consumidor)
                .WithMany(u => u.Reservas)
                .HasForeignKey(r => r.ConsumidorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ConfirmacionRetiro>(entity =>
        {
            entity.HasKey(c => c.ConfirmacionId);
            entity.HasIndex(c => c.ReservaId).IsUnique();
            entity.HasOne(c => c.Reserva)
                .WithOne(r => r.ConfirmacionRetiro)
                .HasForeignKey<ConfirmacionRetiro>(c => c.ReservaId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
