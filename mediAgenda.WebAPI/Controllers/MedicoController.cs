using mediAgenda.Dominio;
using mediAgenda.ILogicaNegocio;
using mediAgenda.WebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mediAgenda.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class MedicoController : ControllerBase
{
    private readonly IMedicoServicio _servicio;

    public MedicoController(IMedicoServicio servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ObtenerTodos()
    {
        var medicos = await _servicio.ObtenerTodosAsync();
        var dto = medicos.Select(m => new MedicoDTO
        {
            Id = m.Id,
            Nombre = m.Nombre,
            Apellido = m.Apellido,
            Email = m.Email,
            EspecialidadId = m.EspecialidadId,
            SedeId = m.SedeId
        });
        return Ok(dto);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var medico = await _servicio.ObtenerPorIdAsync(id);
        if (medico == null) return NotFound();
        return Ok(new MedicoDTO
        {
            Id = medico.Id,
            Nombre = medico.Nombre,
            Apellido = medico.Apellido,
            Email = medico.Email,
            EspecialidadId = medico.EspecialidadId,
            SedeId = medico.SedeId
        });
    }

    [HttpGet("especialidad/{especialidadId}")]
    [AllowAnonymous]
    public async Task<IActionResult> ObtenerPorEspecialidad(int especialidadId)
    {
        var medicos = await _servicio.ObtenerPorEspecialidadAsync(especialidadId);
        var dto = medicos.Select(m => new MedicoDTO
        {
            Id = m.Id,
            Nombre = m.Nombre,
            Apellido = m.Apellido,
            Email = m.Email,
            EspecialidadId = m.EspecialidadId,
            SedeId = m.SedeId
        });
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearMedicoDTO dto)
    {
        var medico = new Medico
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Email = dto.Email,
            EspecialidadId = dto.EspecialidadId,
            SedeId = dto.SedeId
        };
        var creado = await _servicio.CrearAsync(medico);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, new MedicoDTO
        {
            Id = creado.Id,
            Nombre = creado.Nombre,
            Apellido = creado.Apellido,
            Email = creado.Email,
            EspecialidadId = creado.EspecialidadId,
            SedeId = creado.SedeId
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _servicio.EliminarAsync(id);
        return NoContent();
    }
}