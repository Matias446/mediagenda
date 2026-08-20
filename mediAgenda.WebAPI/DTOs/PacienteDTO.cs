namespace mediAgenda.WebAPI.DTOs;

public class PacienteDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public string Cedula { get; set; }
    public string Telefono { get; set; }
}

public class CrearPacienteDTO
{
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Cedula { get; set; }
    public string Telefono { get; set; }
    public DateTime FechaNacimiento { get; set; }
}