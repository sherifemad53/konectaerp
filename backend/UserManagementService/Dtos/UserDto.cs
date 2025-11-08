using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Dtos;

public class UserDto
{
    public Guid Id { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    
    public List<string> Roles { get; set; } = new List<string>();
}
