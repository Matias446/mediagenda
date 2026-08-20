using mediAgenda.Dominio;
using mediAgenda.IDataAccess;
using mediAgenda.ILogicaNegocio;

namespace mediAgenda.LogicaNegocio;

public class PacienteServicio : IPacienteServicio
{
    private readonly IRepositorio<Paciente> _repositorio;

    public PacienteServicio(IRepositorio<Paciente> repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<Paciente>> ObtenerTodosAsync()
        => await _repositorio.ObtenerTodosAsync();

    public async Task<Paciente?> ObtenerPorIdAsync(int id)
        => await _repositorio.ObtenerPorIdAsync(id);

    public async Task<Paciente> CrearAsync(Paciente paciente)
    {
        await _repositorio.AgregarAsync(paciente);
        return paciente;
    }

    public async Task ActualizarAsync(Paciente paciente)
        => await _repositorio.ActualizarAsync(paciente);

    public async Task EliminarAsync(int id)
        => await _repositorio.EliminarAsync(id);
}