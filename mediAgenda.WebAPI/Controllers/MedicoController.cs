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
        if (!string.IsNullOrWhiteSpace(dto.Cedula) && await ExisteOtraCedulaAsync(dto.Cedula, idAExcluir: null))
            throw new InvalidOperationException("Ya existe un médico con esta cédula.");

        var medico = new Medico
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Email = dto.Email,
            Cedula = dto.Cedula,
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarMedicoDTO dto)
    {
        var medico = await _servicio.ObtenerPorIdAsync(id);
        if (medico == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.Cedula) && await ExisteOtraCedulaAsync(dto.Cedula, idAExcluir: id))
            throw new InvalidOperationException("Ya existe otro médico con esta cédula.");

        medico.Nombre = dto.Nombre;
        medico.Apellido = dto.Apellido;
        medico.Email = dto.Email;
        if (!string.IsNullOrWhiteSpace(dto.Cedula))
            medico.Cedula = dto.Cedula;
        medico.EspecialidadId = dto.EspecialidadId;
        medico.SedeId = dto.SedeId;
        await _servicio.ActualizarAsync(medico);

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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _servicio.EliminarAsync(id);
        return NoContent();
    }

    private async Task<bool> ExisteOtraCedulaAsync(string cedula, int? idAExcluir)
    {
        var medicos = await _servicio.ObtenerTodosAsync();
        return medicos.Any(m => m.Cedula == cedula && m.Id != idAExcluir);
    }
}