using System.ComponentModel.DataAnnotations;

namespace mediAgenda.WebAPI.DTOs;

public class TurnoDTO
{
    public int Id { get; set; }
    public int PacienteId { get; set; }
    public string? NombrePaciente { get; set; }
    public int MedicoId { get; set; }
    public string? NombreMedico { get; set; }
    public DateTime FechaHora { get; set; }
    public string Estado { get; set; }
    public string? Motivo { get; set; }
}

public class CrearTurnoDTO
{
    [Required(ErrorMessage = "El paciente es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El paciente es inválido")]
    public int PacienteId { get; set; }

    [Required(ErrorMessage = "El médico es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El médico es inválido")]
    public int MedicoId { get; set; }

    [Required(ErrorMessage = "La fecha y hora es requerida")]
    public DateTime FechaHora { get; set; }

    public string? Motivo { get; set; }
}