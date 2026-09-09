namespace ApiGestion.Modelos;

public class Experiecia
{
    public int Id { get; set; }
    public string NombreCargo { get; set; } = string.Empty;
    public string Institucion { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public DateOnly FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public int Docente { get; set; }
}
