using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using pacientes_service.Communication.Commands;
using pacientes_service.Communication.Services;
using Microsoft.AspNetCore.Identity.Data;
using LoginRequest = pacientes_service.Communication.Commands.LoginRequest;
using RefreshRequest = pacientes_service.Communication.Commands.RefreshRequest;

namespace pacientes_service.API;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult> Login(
        [FromBody] LoginRequest req,
        [FromServices] AuthService service,
        CancellationToken ct)
    {
        var res = await service.LoginAsync(req, ct);
        return res is null ? Unauthorized() : Ok(res);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult> Refresh(
        [FromBody] RefreshRequest req,
        [FromServices] AuthService service,
        CancellationToken ct)
    {
        var res = await service.RefreshAsync(req, ct);
        return res is null ? Unauthorized() : Ok(res);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        [FromBody] RefreshRequest req,
        [FromServices] AuthService service,
        CancellationToken ct)
    {
        await service.LogoutAsync(req.UserId, req.RefreshToken, ct);
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public ActionResult Me()
    {
        return Ok(new
        {
            sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub")?.Value,
            username = User.FindFirstValue("username"),
            role = User.FindFirstValue(ClaimTypes.Role),
            id_medico = User.FindFirstValue("id_medico")
        });
    }
}
