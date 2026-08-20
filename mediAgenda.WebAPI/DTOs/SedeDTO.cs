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
    public string Nombre { get; set; }
    public string Direccion { get; set; }
    public string Telefono { get; set; }
}