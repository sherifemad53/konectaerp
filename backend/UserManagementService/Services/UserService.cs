using UserManagementService.Data;
using UserManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace UserManagementService.Services;

public class UserService : IUserService
{
    private readonly UserDbContext _db;

    public UserService(UserDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _db.Users.FindAsync(id);
    }

    public async Task<User?> GetByIdWithRolesAsync(Guid id)
    {
        return await _db.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _db.Users.ToListAsync();
    }

    public async Task<IEnumerable<User>> GetAllWithRolesAsync()
    {
        return await _db.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .ToListAsync();
    }

    public async Task<User> CreateAsync(User user, List<Guid>? roleIds = null)
    {
        if (user.Id == Guid.Empty)
        {
            user.Id = Guid.NewGuid();
        }

        if (roleIds != null && roleIds.Any())
        {
            var roles = await _db.Roles.Where(r => roleIds.Contains(r.Id)).ToListAsync();
            foreach (var role in roles)
            {
                user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
            }
        }

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<User?> UpdateAsync(Guid id, User user, List<Guid>? roleIds = null)
    {
        var existing = await _db.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (existing == null) return null;

        existing.Username = user.Username;
        existing.Email = user.Email;
        existing.FirstName = user.FirstName;
        existing.LastName = user.LastName;
        existing.IsActive = user.IsActive;

        if (roleIds != null)
        {
            // Remove existing roles
            _db.UserRoles.RemoveRange(existing.UserRoles);

            // Add new roles
            var roles = await _db.Roles.Where(r => roleIds.Contains(r.Id)).ToListAsync();
            foreach (var role in roles)
            {
                existing.UserRoles.Add(new UserRole { UserId = existing.Id, RoleId = role.Id });
            }
        }

        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _db.Users.FindAsync(id);
        if (existing == null) return false;
        _db.Users.Remove(existing);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssignRolesAsync(Guid userId, List<Guid> roleIds)
    {
        var user = await _db.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return false;

        var existingRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();
        var rolesToAdd = roleIds.Except(existingRoleIds);

        var roles = await _db.Roles.Where(r => rolesToAdd.Contains(r.Id)).ToListAsync();
        foreach (var role in roles)
        {
            user.UserRoles.Add(new UserRole { UserId = userId, RoleId = role.Id });
        }

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveRoleAsync(Guid userId, Guid roleId)
    {
        var user = await _db.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return false;

        var userRole = user.UserRoles.FirstOrDefault(ur => ur.RoleId == roleId);
        if (userRole != null)
        {
            _db.UserRoles.Remove(userRole);
            await _db.SaveChangesAsync();
            return true;
        }

        return false;
    }
}
