using mediAgenda.Dominio;

namespace mediAgenda.ILogicaNegocio;

public interface IEspecialidadServicio
{
    Task<IEnumerable<Especialidad>> ObtenerTodosAsync();
    Task<Especialidad?> ObtenerPorIdAsync(int id);
    Task<Especialidad> CrearAsync(Especialidad especialidad);
    Task EliminarAsync(int id);
}