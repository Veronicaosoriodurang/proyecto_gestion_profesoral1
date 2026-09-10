using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class BecaCrear
{
    [Required]
    public int? Estudios { get; set; }

    [Required]
    [MaxLength(45)]
    public string Tipo { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string Institucion { get; set; } = string.Empty;

    [Required]
    public DateOnly? FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }
}
