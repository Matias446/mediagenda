using mediAgenda.Dominio;

namespace mediAgenda.ILogicaNegocio;

public interface IAuthServicio
{
    Task<string?> LoginAsync(string email, string password);
    Task<Usuario> RegistrarPacienteAsync(string email, string password, string cedula, string nombre, string apellido, string telefono, DateTime fechaNacimiento);
    Task CambiarPasswordAsync(int usuarioId, string passwordActual, string passwordNueva);
    Task OlvidePasswordAsync(string email);
}