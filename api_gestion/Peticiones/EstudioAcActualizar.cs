using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class EstudioAcActualizar
{
    public int? Estudio { get; set; }

    [MaxLength(6)]
    public string? AreaConocimiento { get; set; }
}
