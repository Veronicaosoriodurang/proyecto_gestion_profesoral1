using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class RedDocenteCrear
{
    [Required]
    public int? Red { get; set; }

    [Required]
    public int? Docente { get; set; }

    [Required]
    public DateOnly? FechaInicio { get; set; }

    [MaxLength(45)]
    public string? FechaFin { get; set; }

    [Required]
    public string ActDestacadas { get; set; } = string.Empty;
}
