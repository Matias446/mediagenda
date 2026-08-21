namespace mediAgenda.WebAPI.DTOs;

public class LoginDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class RegisterDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Rol { get; set; }
    public int? PacienteId { get; set; }
    public int? MedicoId { get; set; }
}

public class TokenDTO
{
    public string Token { get; set; }
    public string Rol { get; set; }
    public int? PacienteId { get; set; }
    public int? MedicoId { get; set; }
}