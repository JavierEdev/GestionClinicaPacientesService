using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pacientes_service.Communication.Commands;
using pacientes_service.Communication.Contracts;
using pacientes_service.Communication.Services;

namespace pacientes_service.API;

[ApiController]
[Route("api/[controller]")]
[Authorize] // si aún no montas JWT, quítalo temporalmente
public class UsuariosController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll(
        [FromQuery] int? pacienteId,
        [FromServices] ListUsersService service,
        CancellationToken ct)
        => Ok(await service.ExecuteAsync(pacienteId,ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponse>> GetById(
        int id,
        [FromServices] GetUserByIdService service,
        CancellationToken ct)
    {
        var user = await service.ExecuteAsync(id, ct);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    // [Authorize(Roles="administrador")]  // actívalo cuando ya tengas JWT
    public async Task<ActionResult> Create(
        [FromBody] CreateUserCommand cmd,
        [FromServices] CreateUserService service,
        CancellationToken ct)
    {
        try
        {
            var id = await service.ExecuteAsync(cmd, ct); // hace BCrypt.Hash
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{id:int}/delete")]
    // [Authorize(Roles="administrador")]
    public async Task<ActionResult> SoftDelete(
        int id,
        [FromServices] SoftDeleteUserService service,
        CancellationToken ct)
    {
        var ok = await service.ExecuteAsync(new SoftDeleteUserCommand { Id = id, DeletedBy = User.Identity?.Name ?? "system" }, ct);
        return ok ? NoContent() : NotFound();
    }
}
