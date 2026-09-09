using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class ReconocimientoReemplazo
{
    [Required]
    [MaxLength(45)]
    public string Tipo { get; set; } = string.Empty;

    [Required]
    public DateOnly? Fecha { get; set; }

    [Required]
    [MaxLength(45)]
    public string Institucion { get; set; } = string.Empty;

    [Required]
    [MaxLength(45)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [MaxLength(45)]
    public string Ambito { get; set; } = string.Empty;

    [Required]
    public int? Docente { get; set; }
}
