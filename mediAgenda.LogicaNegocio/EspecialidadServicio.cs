using mediAgenda.Dominio;
using mediAgenda.IDataAccess;
using mediAgenda.ILogicaNegocio;

namespace mediAgenda.LogicaNegocio;

public class EspecialidadServicio : IEspecialidadServicio
{
    private readonly IRepositorio<Especialidad> _repositorio;

    public EspecialidadServicio(IRepositorio<Especialidad> repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<Especialidad>> ObtenerTodosAsync()
        => await _repositorio.ObtenerTodosAsync();

    public async Task<Especialidad?> ObtenerPorIdAsync(int id)
        => await _repositorio.ObtenerPorIdAsync(id);

    public async Task<Especialidad> CrearAsync(Especialidad especialidad)
    {
        await _repositorio.AgregarAsync(especialidad);
        return especialidad;
    }

    public async Task EliminarAsync(int id)
        => await _repositorio.EliminarAsync(id);
}