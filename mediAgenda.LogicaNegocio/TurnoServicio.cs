using mediAgenda.Dominio;
using mediAgenda.IDataAccess;
using mediAgenda.ILogicaNegocio;

namespace mediAgenda.LogicaNegocio;

public class TurnoServicio : ITurnoServicio
{
    private readonly IRepositorio<Turno> _repositorio;
    private readonly IRepositorio<Medico> _medicoRepositorio;

    public TurnoServicio(IRepositorio<Turno> repositorio, IRepositorio<Medico> medicoRepositorio)
    {
        _repositorio = repositorio;
        _medicoRepositorio = medicoRepositorio;
    }

    public async Task<IEnumerable<Turno>> ObtenerTodosAsync()
        => await _repositorio.ObtenerTodosAsync();

    public async Task<Turno?> ObtenerPorIdAsync(int id)
        => await _repositorio.ObtenerPorIdAsync(id);

    public async Task<IEnumerable<Turno>> ObtenerPorPacienteAsync(int pacienteId)
    {
        var turnos = await _repositorio.ObtenerTodosAsync();
        return turnos.Where(t => t.PacienteId == pacienteId);
    }

    public async Task<IEnumerable<Turno>> ObtenerPorMedicoAsync(int medicoId)
    {
        var turnos = await _repositorio.ObtenerTodosAsync();
        return turnos.Where(t => t.MedicoId == medicoId);
    }

    public async Task<Turno> CrearAsync(Turno turno)
    {
        turno.Estado = EstadoTurno.Pendiente;
        await _repositorio.AgregarAsync(turno);
        return turno;
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

        var duracion = medico.DuracionTurnoMinutos > 0 ? medico.DuracionTurnoMinutos : 20;

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