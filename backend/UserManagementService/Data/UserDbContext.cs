using Microsoft.EntityFrameworkCore;
using UserManagementService.Models;

namespace UserManagementService.Data;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
        });

        // Role configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // UserRole configuration (many-to-many)
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles");
            entity.HasKey(e => new { e.UserId, e.RoleId });
            
            entity.HasOne(e => e.User)
                  .WithMany(e => e.UserRoles)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.Role)
                  .WithMany(e => e.UserRoles)
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed default roles
        var adminRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var managerRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var employeeRoleId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var seedDate = new DateTime(2025, 1, 1, 0, 0, 0);

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = adminRoleId, Name = "Admin", Description = "System Administrator", CreatedAt = seedDate },
            new Role { Id = managerRoleId, Name = "Manager", Description = "Department Manager", CreatedAt = seedDate },
            new Role { Id = employeeRoleId, Name = "Employee", Description = "Regular Employee", CreatedAt = seedDate }
        );
    }
}
