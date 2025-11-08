using UserManagementService.Models;

namespace UserManagementService.Services;

public interface IRoleService
{
    Task<Role?> GetByIdAsync(Guid id);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role> CreateAsync(Role role);
    Task<Role?> UpdateAsync(Guid id, Role role);
    Task<bool> DeleteAsync(Guid id);
}
