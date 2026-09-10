using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class ApoyoProfesoralReemplazo
{
    [Required]
    public int? Estudios { get; set; }

    [Required]
    public byte? ConApoyo { get; set; }

    [Required]
    [MaxLength(45)]
    public string Institucion { get; set; } = string.Empty;

    [Required]
    [MaxLength(45)]
    public string Tipo { get; set; } = string.Empty;
}
