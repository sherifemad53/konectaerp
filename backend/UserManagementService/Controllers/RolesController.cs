using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementService.Dtos;
using UserManagementService.Services;
using UserManagementService.Models;

namespace UserManagementService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var role = await _roleService.GetByIdAsync(id);
        if (role == null) return NotFound();

        return Ok(new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            CreatedAt = role.CreatedAt
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var roles = await _roleService.GetAllAsync();
        var dtos = roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            CreatedAt = r.CreatedAt
        });
        return Ok(dtos);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] CreateRoleDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };
        
        var created = await _roleService.CreateAsync(role);
        
        return CreatedAtAction(nameof(Get), new { id = created.Id }, new RoleDto
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description,
            CreatedAt = created.CreatedAt
        });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var role = new Role
        {
            Name = dto.Name,
            Description = dto.Description
        };
        
        var updated = await _roleService.UpdateAsync(id, role);
        if (updated == null) return NotFound();
        
        return Ok(new RoleDto
        {
            Id = updated.Id,
            Name = updated.Name,
            Description = updated.Description,
            CreatedAt = updated.CreatedAt
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await _roleService.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}