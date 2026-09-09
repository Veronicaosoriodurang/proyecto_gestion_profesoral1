using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class EstudioAcCrear
{
    [Required]
    public int? Estudio { get; set; }

    [Required]
    [MaxLength(6)]
    public string AreaConocimiento { get; set; } = string.Empty;
}
