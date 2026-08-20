using mediAgenda.Dominio;

namespace mediAgenda.ILogicaNegocio;

public interface IMedicoServicio
{
    Task<IEnumerable<Medico>> ObtenerTodosAsync();
    Task<Medico?> ObtenerPorIdAsync(int id);
    Task<IEnumerable<Medico>> ObtenerPorEspecialidadAsync(int especialidadId);
    Task<Medico> CrearAsync(Medico medico);
    Task ActualizarAsync(Medico medico);
    Task EliminarAsync(int id);
}