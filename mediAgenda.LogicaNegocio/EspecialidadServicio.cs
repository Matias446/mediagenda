using mediAgenda.Dominio;
using mediAgenda.IDataAccess;
using mediAgenda.ILogicaNegocio;

namespace mediAgenda.LogicaNegocio;

public class EspecialidadServicio : IEspecialidadServicio
{
    private readonly IRepositorio<Especialidad> _repositorio;
    private readonly IRepositorio<Medico> _medicoRepositorio;

    public EspecialidadServicio(IRepositorio<Especialidad> repositorio, IRepositorio<Medico> medicoRepositorio)
    {
        _repositorio = repositorio;
        _medicoRepositorio = medicoRepositorio;
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
    {
        var medicos = await _medicoRepositorio.ObtenerTodosAsync();
        if (medicos.Any(m => m.EspecialidadId == id))
            throw new InvalidOperationException("No se puede eliminar una especialidad con médicos asignados");

        await _repositorio.EliminarAsync(id);
    }
}