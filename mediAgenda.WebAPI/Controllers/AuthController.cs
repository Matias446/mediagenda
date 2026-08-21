using mediAgenda.Dominio;
using mediAgenda.ILogicaNegocio;
using mediAgenda.WebAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace mediAgenda.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthServicio _authServicio;

    public AuthController(IAuthServicio authServicio)
    {
        _authServicio = authServicio;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        var token = await _authServicio.LoginAsync(dto.Email, dto.Password);
        if (token == null) return Unauthorized("Email o contraseña incorrectos");
        return Ok(new { token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        if (!Enum.TryParse<RolUsuario>(dto.Rol, out var rol))
            return BadRequest("Rol inválido");

        var usuario = await _authServicio.RegistrarAsync(dto.Email, dto.Password, rol, dto.PacienteId, dto.MedicoId);
        return Ok(new { usuario.Id, usuario.Email, usuario.Rol });
    }
}