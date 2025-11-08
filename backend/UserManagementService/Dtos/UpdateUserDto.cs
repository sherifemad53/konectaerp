using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Dtos;

public class UpdateUserDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    public string LastName { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    public List<Guid> RoleIds { get; set; } = new List<Guid>();
}
