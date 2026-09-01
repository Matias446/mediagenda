namespace mediAgenda.Dominio;

public class Usuario
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public RolUsuario Rol { get; set; }
    public int? PacienteId { get; set; }
    public Paciente? Paciente { get; set; }
}

public enum RolUsuario
{
    Admin = 0,
    Paciente = 1,
    Administrativo = 3
}