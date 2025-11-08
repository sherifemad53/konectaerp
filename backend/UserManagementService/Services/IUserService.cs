using UserManagementService.Models;

namespace UserManagementService.Services;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByIdWithRolesAsync(Guid id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<IEnumerable<User>> GetAllWithRolesAsync();
    Task<User> CreateAsync(User user, List<Guid>? roleIds = null);
    Task<User?> UpdateAsync(Guid id, User user, List<Guid>? roleIds = null);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> AssignRolesAsync(Guid userId, List<Guid> roleIds);
    Task<bool> RemoveRoleAsync(Guid userId, Guid roleId);
}
