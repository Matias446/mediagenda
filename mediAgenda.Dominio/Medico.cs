namespace mediAgenda.Dominio;

public class Medico
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public int EspecialidadId { get; set; }
    public Especialidad Especialidad { get; set; }
    public int SedeId { get; set; }
    public Sede Sede { get; set; }
    public ICollection<Turno> Turnos { get; set; }
    public ICollection<HorarioDisponible> Horarios { get; set; }
}