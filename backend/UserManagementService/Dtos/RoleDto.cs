using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Dtos;

public class RoleDto
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; }
}
