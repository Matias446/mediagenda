using mediAgenda.Dominio;
using mediAgenda.ILogicaNegocio;
using mediAgenda.WebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
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
        var usuario = await _authServicio.RegistrarPacienteAsync(
            dto.Email, dto.Password, dto.Cedula, dto.Nombre, dto.Apellido, dto.Telefono, dto.FechaNacimiento);
        return Ok(new { usuario.Id, usuario.Email, usuario.Rol });
    }

    [HttpPut("cambiar-password")]
    [Authorize]
    public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordDTO dto)
    {
        var usuarioId = int.Parse(User.FindFirst("usuarioId")!.Value);
        await _authServicio.CambiarPasswordAsync(usuarioId, dto.PasswordActual, dto.PasswordNueva);
        return NoContent();
    }

    [HttpPost("olvide-password")]
    [AllowAnonymous]
    public async Task<IActionResult> OlvidePassword([FromBody] OlvidePasswordDTO dto)
    {
        await _authServicio.OlvidePasswordAsync(dto.Email);
        return Ok(new { mensaje = "Si el email existe en el sistema, va a recibir una contraseña temporal." });
    }
}