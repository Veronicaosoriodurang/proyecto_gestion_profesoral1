using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class ExperieciaCrear
{
    [Required]
    [MaxLength(45)]
    public string NombreCargo { get; set; } = string.Empty;

    [Required]
    [MaxLength(45)]
    public string Institucion { get; set; } = string.Empty;

    [Required]
    [MaxLength(45)]
    public string Tipo { get; set; } = string.Empty;

    [Required]
    public DateOnly? FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    [Required]
    public int? Docente { get; set; }
}
