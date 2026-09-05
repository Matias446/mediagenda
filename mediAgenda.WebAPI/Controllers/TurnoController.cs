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
        return Ok(turnos.Select(MapearDto));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var turno = await _servicio.ObtenerPorIdAsync(id);
        if (turno == null) return NotFound();
        if (EsPacienteAjeno(turno.PacienteId)) return Forbid();

        return Ok(MapearDto(turno));
    }

    [HttpGet("paciente/{pacienteId}")]
    [Authorize(Roles = "Paciente,Admin")]
    public async Task<IActionResult> ObtenerPorPaciente(int pacienteId)
    {
        if (EsPacienteAjeno(pacienteId)) return Forbid();

        var turnos = await _servicio.ObtenerPorPacienteAsync(pacienteId);
        return Ok(turnos.Select(MapearDto));
    }

    [HttpGet("medico/{medicoId}")]
    public async Task<IActionResult> ObtenerPorMedico(int medicoId)
    {
        var turnos = await _servicio.ObtenerPorMedicoAsync(medicoId);
        return Ok(turnos.Select(MapearDto));
    }

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
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, MapearDto(creado));
    }

    [HttpPut("{id}/cancelar")]
    public async Task<IActionResult> Cancelar(int id)
    {
        var rol = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "";
        var pacienteIdUsuario = int.TryParse(User.FindFirst("pacienteId")?.Value, out var pid) ? pid : (int?)null;

        await _servicio.CancelarAsync(id, rol, pacienteIdUsuario);
        return NoContent();
    }

    [HttpPut("{id}/confirmar")]
    [Authorize(Roles = "Admin,Administrativo")]
    public async Task<IActionResult> Confirmar(int id)
    {
        await _servicio.ConfirmarAsync(id);
        return NoContent();
    }

    private bool EsPacienteAjeno(int pacienteId)
        => User.IsInRole("Paciente") && User.FindFirst("pacienteId")?.Value != pacienteId.ToString();

    private static TurnoDTO MapearDto(Turno t) => new()
    {
        Id = t.Id,
        PacienteId = t.PacienteId,
        NombrePaciente = t.Paciente != null ? $"{t.Paciente.Nombre} {t.Paciente.Apellido}" : null,
        MedicoId = t.MedicoId,
        NombreMedico = t.Medico != null ? $"{t.Medico.Nombre} {t.Medico.Apellido}" : null,
        FechaHora = t.FechaHora,
        Estado = t.Estado.ToString(),
        Motivo = t.Motivo
    };
}