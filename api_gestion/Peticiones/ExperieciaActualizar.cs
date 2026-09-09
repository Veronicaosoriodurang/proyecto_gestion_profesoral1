using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class ExperieciaActualizar
{
    [MaxLength(45)]
    public string? NombreCargo { get; set; }

    [MaxLength(45)]
    public string? Institucion { get; set; }

    [MaxLength(45)]
    public string? Tipo { get; set; }

    public DateOnly? FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public int? Docente { get; set; }
}
