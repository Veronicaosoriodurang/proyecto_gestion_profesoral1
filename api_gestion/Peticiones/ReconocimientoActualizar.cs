using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class ReconocimientoActualizar
{
    [MaxLength(45)]
    public string? Tipo { get; set; }

    public DateOnly? Fecha { get; set; }

    [MaxLength(45)]
    public string? Institucion { get; set; }

    [MaxLength(45)]
    public string? Nombre { get; set; }

    [MaxLength(45)]
    public string? Ambito { get; set; }

    public int? Docente { get; set; }
}
