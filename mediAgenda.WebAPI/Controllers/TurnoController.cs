using mediAgenda.Dominio;
using mediAgenda.ILogicaNegocio;
using mediAgenda.WebAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace mediAgenda.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TurnoController : ControllerBase
{
    private readonly ITurnoServicio _servicio;

    public TurnoController(ITurnoServicio servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var turnos = await _servicio.ObtenerTodosAsync();
        var dto = turnos.Select(t => new TurnoDTO
        {
            Id = t.Id,
            PacienteId = t.PacienteId,
            MedicoId = t.MedicoId,
            FechaHora = t.FechaHora,
            Estado = t.Estado.ToString(),
            Motivo = t.Motivo
        });
        return Ok(dto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var turno = await _servicio.ObtenerPorIdAsync(id);
        if (turno == null) return NotFound();
        return Ok(new TurnoDTO
        {
            Id = turno.Id,
            PacienteId = turno.PacienteId,
            MedicoId = turno.MedicoId,
            FechaHora = turno.FechaHora,
            Estado = turno.Estado.ToString(),
            Motivo = turno.Motivo
        });
    }

    [HttpGet("paciente/{pacienteId}")]
    public async Task<IActionResult> ObtenerPorPaciente(int pacienteId)
    {
        var turnos = await _servicio.ObtenerPorPacienteAsync(pacienteId);
        var dto = turnos.Select(t => new TurnoDTO
        {
            Id = t.Id,
            PacienteId = t.PacienteId,
            MedicoId = t.MedicoId,
            FechaHora = t.FechaHora,
            Estado = t.Estado.ToString(),
            Motivo = t.Motivo
        });
        return Ok(dto);
    }

    [HttpGet("medico/{medicoId}")]
    public async Task<IActionResult> ObtenerPorMedico(int medicoId)
    {
        var turnos = await _servicio.ObtenerPorMedicoAsync(medicoId);
        var dto = turnos.Select(t => new TurnoDTO
        {
            Id = t.Id,
            PacienteId = t.PacienteId,
            MedicoId = t.MedicoId,
            FechaHora = t.FechaHora,
            Estado = t.Estado.ToString(),
            Motivo = t.Motivo
        });
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearTurnoDTO dto)
    {
        var turno = new Turno
        {
            PacienteId = dto.PacienteId,
            MedicoId = dto.MedicoId,
            FechaHora = dto.FechaHora,
            Motivo = dto.Motivo
        };
        var creado = await _servicio.CrearAsync(turno);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, new TurnoDTO
        {
            Id = creado.Id,
            PacienteId = creado.PacienteId,
            MedicoId = creado.MedicoId,
            FechaHora = creado.FechaHora,
            Estado = creado.Estado.ToString(),
            Motivo = creado.Motivo
        });
    }

    [HttpPut("{id}/cancelar")]
    public async Task<IActionResult> Cancelar(int id)
    {
        await _servicio.CancelarAsync(id);
        return NoContent();
    }
}