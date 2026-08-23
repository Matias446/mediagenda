using System.ComponentModel.DataAnnotations;

namespace mediAgenda.WebAPI.DTOs;

public class EspecialidadDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; }
}

public class CrearEspecialidadDTO
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MinLength(3, ErrorMessage = "El nombre debe tener al menos 3 caracteres")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    public string Nombre { get; set; }
}