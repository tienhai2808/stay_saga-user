using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var user = modelBuilder.Entity<User>();
        user.ToTable("users");

        user.HasKey(x => x.Id);
        user.Property(x => x.Id)
          .HasColumnName("id")
          .ValueGeneratedNever();

        user.Property(x => x.KeycloakId)
          .HasColumnName("keycloak_id")
          .IsRequired()
          .HasMaxLength(64)
          .HasColumnType("character varying(64)");

        user.Property(x => x.Email)
          .HasColumnName("email")
          .IsRequired()
          .HasMaxLength(255)
          .HasColumnType("character varying(255)");

        user.Property(x => x.FirstName)
          .HasColumnName("first_name")
          .IsRequired()
          .HasMaxLength(100)
          .HasColumnType("character varying(100)");

        user.Property(x => x.LastName)
          .HasColumnName("last_name")
          .IsRequired()
          .HasMaxLength(100)
          .HasColumnType("character varying(100)");

        user.Property(x => x.Phone)
          .HasColumnName("phone")
          .IsRequired()
          .HasMaxLength(32)
          .HasColumnType("character varying(32)");

        user.Property(x => x.CreatedAt)
          .HasColumnName("created_at")
          .IsRequired()
          .HasColumnType("timestamp with time zone");

        user.Property(x => x.UpdatedAt)
          .HasColumnName("updated_at")
          .HasColumnType("timestamp with time zone");

        user.HasIndex(x => x.Email)
          .IsUnique()
          .HasDatabaseName("ux_users_email");

        user.HasIndex(x => x.KeycloakId)
          .IsUnique()
          .HasDatabaseName("ux_users_keycloak_id");
    }
}
