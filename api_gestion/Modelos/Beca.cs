namespace ApiGestion.Modelos;

public class Beca
{
    public int Estudios { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Institucion { get; set; } = string.Empty;
    public DateOnly FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
}
