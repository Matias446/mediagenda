using System.ComponentModel.DataAnnotations;

namespace mediAgenda.WebAPI.DTOs;

public class LoginDTO
{
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Password { get; set; }
}

public class RegisterDTO
{
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Password { get; set; }

    [Required(ErrorMessage = "El rol es requerido")]
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