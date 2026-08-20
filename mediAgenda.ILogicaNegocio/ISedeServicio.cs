using mediAgenda.Dominio;

namespace mediAgenda.ILogicaNegocio;

public interface ISedeServicio
{
    Task<IEnumerable<Sede>> ObtenerTodosAsync();
    Task<Sede?> ObtenerPorIdAsync(int id);
    Task<Sede> CrearAsync(Sede sede);
    Task EliminarAsync(int id);
}