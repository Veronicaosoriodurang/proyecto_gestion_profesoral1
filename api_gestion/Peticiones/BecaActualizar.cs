using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class BecaActualizar
{
    public int? Estudios { get; set; }

    [MaxLength(45)]
    public string? Tipo { get; set; }

    [MaxLength(80)]
    public string? Institucion { get; set; }

    public DateOnly? FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }
}
