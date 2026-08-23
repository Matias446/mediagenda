using mediAgenda.Dominio;
using mediAgenda.ILogicaNegocio;
using mediAgenda.WebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mediAgenda.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class PacienteController : ControllerBase
{
    private readonly IPacienteServicio _servicio;

    public PacienteController(IPacienteServicio servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var pacientes = await _servicio.ObtenerTodosAsync();
        var dto = pacientes.Select(p => new PacienteDTO
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Apellido = p.Apellido,
            Email = p.Email,
            Cedula = p.Cedula,
            Telefono = p.Telefono
        });
        return Ok(dto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var paciente = await _servicio.ObtenerPorIdAsync(id);
        if (paciente == null) return NotFound();
        return Ok(new PacienteDTO
        {
            Id = paciente.Id,
            Nombre = paciente.Nombre,
            Apellido = paciente.Apellido,
            Email = paciente.Email,
            Cedula = paciente.Cedula,
            Telefono = paciente.Telefono
        });
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Crear([FromBody] CrearPacienteDTO dto)
    {
        var paciente = new Paciente
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Cedula = dto.Cedula,
            Telefono = dto.Telefono,
            FechaNacimiento = DateTime.SpecifyKind(dto.FechaNacimiento, DateTimeKind.Utc)
        };
        var creado = await _servicio.CrearAsync(paciente);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, new PacienteDTO
        {
            Id = creado.Id,
            Nombre = creado.Nombre,
            Apellido = creado.Apellido,
            Email = creado.Email,
            Cedula = creado.Cedula,
            Telefono = creado.Telefono
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _servicio.EliminarAsync(id);
        return NoContent();
    }
}