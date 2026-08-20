namespace mediAgenda.WebAPI.DTOs;

public class TurnoDTO
{
    public int Id { get; set; }
    public int PacienteId { get; set; }
    public int MedicoId { get; set; }
    public DateTime FechaHora { get; set; }
    public string Estado { get; set; }
    public string? Motivo { get; set; }
}

public class CrearTurnoDTO
{
    public int PacienteId { get; set; }
    public int MedicoId { get; set; }
    public DateTime FechaHora { get; set; }
    public string? Motivo { get; set; }
}