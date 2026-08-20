using mediAgenda.Dominio;
using mediAgenda.IDataAccess;
using mediAgenda.ILogicaNegocio;

namespace mediAgenda.LogicaNegocio;

public class SedeServicio : ISedeServicio
{
    private readonly IRepositorio<Sede> _repositorio;

    public SedeServicio(IRepositorio<Sede> repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<Sede>> ObtenerTodosAsync()
        => await _repositorio.ObtenerTodosAsync();

    public async Task<Sede?> ObtenerPorIdAsync(int id)
        => await _repositorio.ObtenerPorIdAsync(id);

    public async Task<Sede> CrearAsync(Sede sede)
    {
        await _repositorio.AgregarAsync(sede);
        return sede;
    }

    public async Task EliminarAsync(int id)
        => await _repositorio.EliminarAsync(id);
}