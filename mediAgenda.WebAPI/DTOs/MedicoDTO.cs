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
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public int EspecialidadId { get; set; }
    public int SedeId { get; set; }
}