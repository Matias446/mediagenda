using mediAgenda.Dominio;

namespace mediAgenda.ILogicaNegocio;

public interface IPacienteServicio
{
    Task<IEnumerable<Paciente>> ObtenerTodosAsync();
    Task<Paciente?> ObtenerPorIdAsync(int id);
    Task<Paciente> CrearAsync(Paciente paciente);
    Task ActualizarAsync(Paciente paciente);
    Task EliminarAsync(int id);
    Task ActualizarPerfilAsync(int pacienteId, string nombre, string apellido, string telefono);
    Task CambiarPasswordAsync(int pacienteId, string passwordActual, string passwordNueva);
}