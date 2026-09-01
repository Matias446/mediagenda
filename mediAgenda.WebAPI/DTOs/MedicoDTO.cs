using System.ComponentModel.DataAnnotations;

namespace mediAgenda.WebAPI.DTOs;

public class MedicoDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public int EspecialidadId { get; set; }
    public int SedeId { get; set; }
}

public class CrearMedicoDTO
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El apellido es requerido")]
    [MaxLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres")]
    public string Apellido { get; set; }

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; }

    [MaxLength(20, ErrorMessage = "La cédula no puede superar los 20 caracteres")]
    public string? Cedula { get; set; }

    [Required(ErrorMessage = "La especialidad es requerida")]
    [Range(1, int.MaxValue, ErrorMessage = "La especialidad es inválida")]
    public int EspecialidadId { get; set; }

    [Required(ErrorMessage = "La sede es requerida")]
    [Range(1, int.MaxValue, ErrorMessage = "La sede es inválida")]
    public int SedeId { get; set; }
}

public class ActualizarMedicoDTO
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El apellido es requerido")]
    [MaxLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres")]
    public string Apellido { get; set; }

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; }

    [MaxLength(20, ErrorMessage = "La cédula no puede superar los 20 caracteres")]
    public string? Cedula { get; set; }

    [Required(ErrorMessage = "La especialidad es requerida")]
    [Range(1, int.MaxValue, ErrorMessage = "La especialidad es inválida")]
    public int EspecialidadId { get; set; }

    [Required(ErrorMessage = "La sede es requerida")]
    [Range(1, int.MaxValue, ErrorMessage = "La sede es inválida")]
    public int SedeId { get; set; }
}