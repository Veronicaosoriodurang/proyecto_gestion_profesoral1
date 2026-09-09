using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class RedDocenteActualizar
{
    public int? Red { get; set; }

    public int? Docente { get; set; }

    public DateOnly? FechaInicio { get; set; }

    [MaxLength(45)]
    public string? FechaFin { get; set; }

    public string? ActDestacadas { get; set; }
}
