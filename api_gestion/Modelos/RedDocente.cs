namespace ApiGestion.Modelos;

public class RedDocente
{
    public int Red { get; set; }
    public int Docente { get; set; }
    public DateOnly FechaInicio { get; set; }
    public string? FechaFin { get; set; }
    public string ActDestacadas { get; set; } = string.Empty;
}
