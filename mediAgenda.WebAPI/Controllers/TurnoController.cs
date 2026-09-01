using mediAgenda.Dominio;
using mediAgenda.ILogicaNegocio;
using mediAgenda.WebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mediAgenda.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TurnoController : ControllerBase
{
    private readonly ITurnoServicio _servicio;

    public TurnoController(ITurnoServicio servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Administrativo")]
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
        if (EsPacienteAjeno(turno.PacienteId)) return Forbid();

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
    [Authorize(Roles = "Paciente,Admin")]
    public async Task<IActionResult> ObtenerPorPaciente(int pacienteId)
    {
        if (EsPacienteAjeno(pacienteId)) return Forbid();

        return Ok(await _servicio.ObtenerPorPacienteAsync(pacienteId));
    }

    [HttpGet("medico/{medicoId}")]
    public async Task<IActionResult> ObtenerPorMedico(int medicoId)
        => Ok(await _servicio.ObtenerPorMedicoAsync(medicoId));

    [HttpGet("disponibles")]
    [AllowAnonymous]
    public async Task<IActionResult> ObtenerSlotsDisponibles([FromQuery] int medicoId, [FromQuery] DateTime fecha)
    {
        var fechaUtc = DateTime.SpecifyKind(fecha, DateTimeKind.Utc);
        var slots = await _servicio.ObtenerSlotsDisponiblesAsync(medicoId, fechaUtc);
        return Ok(slots);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearTurnoDTO dto)
    {
        if (EsPacienteAjeno(dto.PacienteId)) return Forbid();

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
        var turno = await _servicio.ObtenerPorIdAsync(id);
        if (turno == null) return NotFound();
        if (EsPacienteAjeno(turno.PacienteId)) return Forbid();

        await _servicio.CancelarAsync(id);
        return NoContent();
    }

    private bool EsPacienteAjeno(int pacienteId)
        => User.IsInRole("Paciente") && User.FindFirst("pacienteId")?.Value != pacienteId.ToString();
}