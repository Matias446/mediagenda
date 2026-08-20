using mediAgenda.Dominio;
using mediAgenda.IDataAccess;
using mediAgenda.ILogicaNegocio;

namespace mediAgenda.LogicaNegocio;

public class MedicoServicio : IMedicoServicio
{
    private readonly IRepositorio<Medico> _repositorio;

    public MedicoServicio(IRepositorio<Medico> repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<Medico>> ObtenerTodosAsync()
        => await _repositorio.ObtenerTodosAsync();

    public async Task<Medico?> ObtenerPorIdAsync(int id)
        => await _repositorio.ObtenerPorIdAsync(id);

    public async Task<IEnumerable<Medico>> ObtenerPorEspecialidadAsync(int especialidadId)
    {
        var medicos = await _repositorio.ObtenerTodosAsync();
        return medicos.Where(m => m.EspecialidadId == especialidadId);
    }

    public async Task<Medico> CrearAsync(Medico medico)
    {
        await _repositorio.AgregarAsync(medico);
        return medico;
    }

    public async Task ActualizarAsync(Medico medico)
        => await _repositorio.ActualizarAsync(medico);

    public async Task EliminarAsync(int id)
        => await _repositorio.EliminarAsync(id);
}