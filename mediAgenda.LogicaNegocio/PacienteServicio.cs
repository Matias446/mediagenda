using mediAgenda.Dominio;
using mediAgenda.IDataAccess;
using mediAgenda.ILogicaNegocio;

namespace mediAgenda.LogicaNegocio;

public class PacienteServicio : IPacienteServicio
{
    private readonly IRepositorio<Paciente> _repositorio;
    private readonly IRepositorio<Usuario> _usuarioRepositorio;

    public PacienteServicio(IRepositorio<Paciente> repositorio, IRepositorio<Usuario> usuarioRepositorio)
    {
        _repositorio = repositorio;
        _usuarioRepositorio = usuarioRepositorio;
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
    {
        var usuarios = await _usuarioRepositorio.ObtenerTodosAsync();
        var usuario = usuarios.FirstOrDefault(u => u.PacienteId == id);
        if (usuario != null)
            await _usuarioRepositorio.EliminarAsync(usuario.Id);

        await _repositorio.EliminarAsync(id);
    }

    public async Task ActualizarPerfilAsync(int pacienteId, string nombre, string apellido, string telefono)
    {
        var paciente = await _repositorio.ObtenerPorIdAsync(pacienteId);
        if (paciente == null) throw new KeyNotFoundException("Paciente no encontrado");

        paciente.Nombre = nombre;
        paciente.Apellido = apellido;
        paciente.Telefono = telefono;
        await _repositorio.ActualizarAsync(paciente);
    }

    public async Task CambiarPasswordAsync(int pacienteId, string passwordActual, string passwordNueva)
    {
        var usuarios = await _usuarioRepositorio.ObtenerTodosAsync();
        var usuario = usuarios.FirstOrDefault(u => u.PacienteId == pacienteId);
        if (usuario == null) throw new KeyNotFoundException("Usuario no encontrado");
        if (!BCrypt.Net.BCrypt.Verify(passwordActual, usuario.Password))
            throw new InvalidOperationException("La contraseña actual es incorrecta");

        var nuevoHash = BCrypt.Net.BCrypt.HashPassword(passwordNueva);
        usuario.Password = nuevoHash;
        usuario.TokensValidosDesde = DateTime.UtcNow;
        await _usuarioRepositorio.ActualizarAsync(usuario);

        var paciente = await _repositorio.ObtenerPorIdAsync(pacienteId);
        if (paciente != null)
        {
            paciente.Password = nuevoHash;
            await _repositorio.ActualizarAsync(paciente);
        }
    }
}