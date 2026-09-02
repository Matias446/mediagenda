namespace mediAgenda.Dominio;

public class Usuario
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public RolUsuario Rol { get; set; }
    public int? PacienteId { get; set; }
    public Paciente? Paciente { get; set; }

    /// <summary>
    /// Cualquier JWT emitido con "iat" anterior a esta fecha se rechaza.
    /// Se actualiza al eliminar la cuenta o cambiar/resetear la contraseña.
    /// Null = sin restricción (cuentas creadas antes de este campo).
    /// </summary>
    public DateTime? TokensValidosDesde { get; set; }
}

public enum RolUsuario
{
    Admin = 0,
    Paciente = 1,
    Administrativo = 3
}