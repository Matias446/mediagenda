namespace mediAgenda.Dominio;

public class Paciente
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Cedula { get; set; }
    public string Telefono { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public ICollection<Turno> Turnos { get; set; }
}