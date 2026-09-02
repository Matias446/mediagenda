using mediAgenda.Dominio;
using mediAgenda.IDataAccess;
using mediAgenda.ILogicaNegocio;

namespace mediAgenda.LogicaNegocio;

public class SedeServicio : ISedeServicio
{
    private readonly IRepositorio<Sede> _repositorio;
    private readonly IRepositorio<Medico> _medicoRepositorio;

    public SedeServicio(IRepositorio<Sede> repositorio, IRepositorio<Medico> medicoRepositorio)
    {
        _repositorio = repositorio;
        _medicoRepositorio = medicoRepositorio;
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
    {
        var medicos = await _medicoRepositorio.ObtenerTodosAsync();
        if (medicos.Any(m => m.SedeId == id))
            throw new InvalidOperationException("No se puede eliminar una sede con médicos asignados");

        await _repositorio.EliminarAsync(id);
    }
}