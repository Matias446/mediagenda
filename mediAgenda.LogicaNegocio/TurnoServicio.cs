using mediAgenda.Dominio;
using mediAgenda.IDataAccess;
using mediAgenda.ILogicaNegocio;
using Microsoft.Extensions.Logging;

namespace mediAgenda.LogicaNegocio;

public class TurnoServicio : ITurnoServicio
{
    private readonly IRepositorio<Turno> _repositorio;
    private readonly IRepositorio<Medico> _medicoRepositorio;
    private readonly IRepositorio<Paciente> _pacienteRepositorio;
    private readonly ILogger<TurnoServicio> _logger;

    public TurnoServicio(IRepositorio<Turno> repositorio, IRepositorio<Medico> medicoRepositorio, IRepositorio<Paciente> pacienteRepositorio, ILogger<TurnoServicio> logger)
    {
        _repositorio = repositorio;
        _medicoRepositorio = medicoRepositorio;
        _pacienteRepositorio = pacienteRepositorio;
        _logger = logger;
    }

    private const int DiasVisibilidadCancelados = 30;

    private static bool EsVisibleEnListado(Turno t)
        => t.Estado != EstadoTurno.Cancelado || t.FechaHora >= DateTime.UtcNow.AddDays(-DiasVisibilidadCancelados);

    public async Task<IEnumerable<Turno>> ObtenerTodosAsync()
    {
        var turnos = (await _repositorio.ObtenerTodosAsync()).Where(EsVisibleEnListado);
        return await ConNombresAsync(turnos);
    }

    public async Task<Turno?> ObtenerPorIdAsync(int id)
    {
        var turno = await _repositorio.ObtenerPorIdAsync(id);
        if (turno == null) return null;
        return (await ConNombresAsync(new[] { turno })).First();
    }

    public async Task<IEnumerable<Turno>> ObtenerPorPacienteAsync(int pacienteId)
    {
        var turnos = await _repositorio.ObtenerTodosAsync();
        return await ConNombresAsync(turnos.Where(t => t.PacienteId == pacienteId).Where(EsVisibleEnListado));
    }

    public async Task<IEnumerable<Turno>> ObtenerPorMedicoAsync(int medicoId)
    {
        var turnos = await _repositorio.ObtenerTodosAsync();
        return await ConNombresAsync(turnos.Where(t => t.MedicoId == medicoId).Where(EsVisibleEnListado));
    }

    public async Task<Turno> CrearAsync(Turno turno)
    {
        if (turno.FechaHora <= DateTime.UtcNow)
            throw new InvalidOperationException("No se puede reservar un turno en una fecha pasada");

        var turnosExistentes = await _repositorio.ObtenerTodosAsync();
        var yaReservado = turnosExistentes.Any(t =>
            t.PacienteId == turno.PacienteId
            && t.MedicoId == turno.MedicoId
            && t.FechaHora == turno.FechaHora
            && (t.Estado == EstadoTurno.Pendiente || t.Estado == EstadoTurno.Confirmado));
        if (yaReservado)
            throw new InvalidOperationException("Ya tenés un turno reservado en ese horario");

        turno.Estado = EstadoTurno.Pendiente;
        await _repositorio.AgregarAsync(turno);
        return (await ConNombresAsync(new[] { turno })).First();
    }

    private async Task<IEnumerable<Turno>> ConNombresAsync(IEnumerable<Turno> turnos)
    {
        var lista = turnos.ToList();
        if (lista.Count == 0) return lista;

        var pacientes = (await _pacienteRepositorio.ObtenerTodosAsync()).ToDictionary(p => p.Id);
        var medicos = (await _medicoRepositorio.ObtenerTodosAsync()).ToDictionary(m => m.Id);

        foreach (var turno in lista)
        {
            pacientes.TryGetValue(turno.PacienteId, out var paciente);
            turno.Paciente = paciente!;
            medicos.TryGetValue(turno.MedicoId, out var medico);
            turno.Medico = medico!;
        }
        return lista;
    }

    public async Task CancelarAsync(int id)
    {
        var turno = await _repositorio.ObtenerPorIdAsync(id);
        if (turno == null) throw new Exception("Turno no encontrado");
        turno.Estado = EstadoTurno.Cancelado;
        await _repositorio.ActualizarAsync(turno);
    }

    public async Task<IEnumerable<DateTime>> ObtenerSlotsDisponiblesAsync(int medicoId, DateTime fecha)
    {
        var medico = await _medicoRepositorio.ObtenerPorIdAsync(medicoId);
        if (medico == null) return Enumerable.Empty<DateTime>();

        if (medico.DuracionTurnoMinutos <= 0)
        {
            _logger.LogWarning(
                "Médico {MedicoId} tiene DuracionTurnoMinutos={Duracion}; no se pueden calcular slots.",
                medicoId, medico.DuracionTurnoMinutos);
            return Enumerable.Empty<DateTime>();
        }

        var duracion = medico.DuracionTurnoMinutos;

        var turnosDelDia = (await _repositorio.ObtenerTodosAsync())
            .Where(t => t.MedicoId == medicoId
                        && t.FechaHora.Date == fecha.Date
                        && t.Estado != EstadoTurno.Cancelado)
            .Select(t => t.FechaHora)
            .ToHashSet();

        var slots = new List<DateTime>();

        var inicio = fecha.Date.AddHours(7);
        var fin = fecha.Date.AddHours(12);
        while (inicio < fin)
        {
            if (!turnosDelDia.Contains(inicio))
                slots.Add(inicio);
            inicio = inicio.AddMinutes(duracion);
        }

        inicio = fecha.Date.AddHours(14);
        fin = fecha.Date.AddHours(18);
        while (inicio < fin)
        {
            if (!turnosDelDia.Contains(inicio))
                slots.Add(inicio);
            inicio = inicio.AddMinutes(duracion);
        }

        return slots;
    }
}