using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementService.Dtos;
using UserManagementService.Services;
using UserManagementService.Models;
using System.Security.Claims;

namespace UserManagementService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var user = await _userService.GetByIdWithRolesAsync(id);
        if (user == null) return NotFound();

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };

        return Ok(userDto);
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllWithRolesAsync();
        var dtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            LastLoginAt = u.LastLoginAt,
            Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
        });
        return Ok(dtos);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = dto.Username,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _userService.CreateAsync(user, dto.RoleIds);
        
        // Get created user with roles to return
        var createdUser = await _userService.GetByIdWithRolesAsync(created.Id);
        
        var userDto = new UserDto
        {
            Id = createdUser!.Id,
            Username = createdUser.Username,
            Email = createdUser.Email,
            FirstName = createdUser.FirstName,
            LastName = createdUser.LastName,
            IsActive = createdUser.IsActive,
            CreatedAt = createdUser.CreatedAt,
            LastLoginAt = createdUser.LastLoginAt,
            Roles = createdUser.UserRoles.Select(ur => ur.Role.Name).ToList()
        };

        return CreatedAtAction(nameof(Get), new { id = userDto.Id }, userDto);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            IsActive = dto.IsActive
        };

        var updated = await _userService.UpdateAsync(id, user, dto.RoleIds);
        if (updated == null) return NotFound();

        // Get updated user with roles to return
        var updatedUser = await _userService.GetByIdWithRolesAsync(id);
        
        var userDto = new UserDto
        {
            Id = updatedUser!.Id,
            Username = updatedUser.Username,
            Email = updatedUser.Email,
            FirstName = updatedUser.FirstName,
            LastName = updatedUser.LastName,
            IsActive = updatedUser.IsActive,
            CreatedAt = updatedUser.CreatedAt,
            LastLoginAt = updatedUser.LastLoginAt,
            Roles = updatedUser.UserRoles.Select(ur => ur.Role.Name).ToList()
        };

        return Ok(userDto);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await _userService.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpPost("{id}/roles")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> AssignRoles(Guid id, [FromBody] AssignRolesDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var success = await _userService.AssignRolesAsync(id, dto.RoleIds);
        if (!success) return NotFound();

        return Ok(new { message = "Roles assigned successfully" });
    }

    [HttpDelete("{id}/roles/{roleId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> RemoveRole(Guid id, Guid roleId)
    {
        var success = await _userService.RemoveRoleAsync(id, roleId);
        if (!success) return NotFound();

        return Ok(new { message = "Role removed successfully" });
    }
}

public class AssignRolesDto
{
    public List<Guid> RoleIds { get; set; } = new List<Guid>();
}