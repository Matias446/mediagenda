using System.ComponentModel.DataAnnotations;

namespace mediAgenda.WebAPI.DTOs;

public class SedeDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Direccion { get; set; }
    public string Telefono { get; set; }
}

public class CrearSedeDTO
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MinLength(3, ErrorMessage = "El nombre debe tener al menos 3 caracteres")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "La dirección es requerida")]
    [MaxLength(200, ErrorMessage = "La dirección no puede superar los 200 caracteres")]
    public string Direccion { get; set; }

    [Required(ErrorMessage = "El teléfono es requerido")]
    [Phone(ErrorMessage = "Teléfono inválido")]
    public string Telefono { get; set; }
}