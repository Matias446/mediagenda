namespace mediAgenda.Dominio;

public class HorarioDisponible
{
    public int Id { get; set; }
    public int MedicoId { get; set; }
    public Medico Medico { get; set; }
    public DayOfWeek DiaSemana { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }
}