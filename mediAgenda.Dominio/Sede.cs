namespace mediAgenda.Dominio;

public class Sede
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Direccion { get; set; }
    public string Telefono { get; set; }
    public ICollection<Medico> Medicos { get; set; }
}