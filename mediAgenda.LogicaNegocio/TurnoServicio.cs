using mediAgenda.Dominio;
using mediAgenda.IDataAccess;
using mediAgenda.ILogicaNegocio;

namespace mediAgenda.LogicaNegocio;

public class TurnoServicio : ITurnoServicio
{
    private readonly IRepositorio<Turno> _repositorio;

    public TurnoServicio(IRepositorio<Turno> repositorio)
    {
        _repositorio = repositorio;
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
}