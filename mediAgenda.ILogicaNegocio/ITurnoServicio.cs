using mediAgenda.Dominio;

namespace mediAgenda.ILogicaNegocio;

public interface ITurnoServicio
{
    Task<IEnumerable<Turno>> ObtenerTodosAsync();
    Task<Turno?> ObtenerPorIdAsync(int id);
    Task<IEnumerable<Turno>> ObtenerPorPacienteAsync(int pacienteId);
    Task<IEnumerable<Turno>> ObtenerPorMedicoAsync(int medicoId);
    Task<Turno> CrearAsync(Turno turno);
    Task CancelarAsync(int id, string rolUsuario, int? pacienteIdUsuario);
    Task<IEnumerable<DateTime>> ObtenerSlotsDisponiblesAsync(int medicoId, DateTime fecha);
}