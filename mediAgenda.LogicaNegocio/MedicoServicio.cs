using mediAgenda.Dominio;
using mediAgenda.IDataAccess;
using mediAgenda.ILogicaNegocio;

namespace mediAgenda.LogicaNegocio;

public class MedicoServicio : IMedicoServicio
{
    private readonly IRepositorio<Medico> _repositorio;
    private readonly IRepositorio<Especialidad> _especialidadRepositorio;
    private readonly IRepositorio<Sede> _sedeRepositorio;

    public MedicoServicio(IRepositorio<Medico> repositorio, IRepositorio<Especialidad> especialidadRepositorio, IRepositorio<Sede> sedeRepositorio)
    {
        _repositorio = repositorio;
        _especialidadRepositorio = especialidadRepositorio;
        _sedeRepositorio = sedeRepositorio;
    }

    public async Task<IEnumerable<Medico>> ObtenerTodosAsync()
        => await ConNavegacionAsync(await _repositorio.ObtenerTodosAsync());

    public async Task<Medico?> ObtenerPorIdAsync(int id)
    {
        var medico = await _repositorio.ObtenerPorIdAsync(id);
        if (medico == null) return null;
        return (await ConNavegacionAsync(new[] { medico })).First();
    }

    public async Task<IEnumerable<Medico>> ObtenerPorEspecialidadAsync(int especialidadId)
    {
        var medicos = await _repositorio.ObtenerTodosAsync();
        return await ConNavegacionAsync(medicos.Where(m => m.EspecialidadId == especialidadId));
    }

    public async Task<Medico> CrearAsync(Medico medico)
    {
        await _repositorio.AgregarAsync(medico);
        return (await ConNavegacionAsync(new[] { medico })).First();
    }

    public async Task ActualizarAsync(Medico medico)
        => await _repositorio.ActualizarAsync(medico);

    public async Task EliminarAsync(int id)
        => await _repositorio.EliminarAsync(id);

    private async Task<IEnumerable<Medico>> ConNavegacionAsync(IEnumerable<Medico> medicos)
    {
        var lista = medicos.ToList();
        if (lista.Count == 0) return lista;

        var especialidades = (await _especialidadRepositorio.ObtenerTodosAsync()).ToDictionary(e => e.Id);
        var sedes = (await _sedeRepositorio.ObtenerTodosAsync()).ToDictionary(s => s.Id);

        foreach (var medico in lista)
        {
            especialidades.TryGetValue(medico.EspecialidadId, out var especialidad);
            medico.Especialidad = especialidad!;
            sedes.TryGetValue(medico.SedeId, out var sede);
            medico.Sede = sede!;
        }
        return lista;
    }
}