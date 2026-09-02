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

    [Required(ErrorMessage = "Debe confirmar la contraseña")]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "La cédula es requerida")]
    [MaxLength(20, ErrorMessage = "La cédula no puede superar los 20 caracteres")]
    public string Cedula { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El apellido es requerido")]
    [MaxLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres")]
    public string Apellido { get; set; }

    [Required(ErrorMessage = "El teléfono es requerido")]
    [Phone(ErrorMessage = "Teléfono inválido")]
    public string Telefono { get; set; }

    [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
    public DateTime FechaNacimiento { get; set; }
}

public class TokenDTO
{
    public string Token { get; set; }
    public string Rol { get; set; }
    public int? PacienteId { get; set; }
}

public class CambiarPasswordDTO
{
    [Required(ErrorMessage = "La contraseña actual es requerida")]
    public string PasswordActual { get; set; }

    [Required(ErrorMessage = "La contraseña nueva es requerida")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string PasswordNueva { get; set; }

    [Required(ErrorMessage = "Debe confirmar la contraseña nueva")]
    [Compare(nameof(PasswordNueva), ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmarPasswordNueva { get; set; }
}

public class OlvidePasswordDTO
{
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; }
}