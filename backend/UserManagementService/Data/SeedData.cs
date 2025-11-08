using UserManagementService.Models;

namespace UserManagementService.Data;

public static class SeedData
{
    public static void Initialize(UserDbContext db)
    {
        // Roles are seeded in OnModelCreating, so we just ensure the database is set up
        if (db.Roles != null && !db.Roles.Any())
        {
            db.SaveChanges();
        }
    }
}
