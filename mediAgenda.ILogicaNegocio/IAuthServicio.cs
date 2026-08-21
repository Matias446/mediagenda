using mediAgenda.Dominio;

namespace mediAgenda.ILogicaNegocio;

public interface IAuthServicio
{
    Task<string?> LoginAsync(string email, string password);
    Task<Usuario> RegistrarAsync(string email, string password, RolUsuario rol, int? pacienteId, int? medicoId);
}