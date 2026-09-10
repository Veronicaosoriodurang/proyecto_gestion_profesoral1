using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class ApoyoProfesoralActualizar
{
    public int? Estudios { get; set; }

    public byte? ConApoyo { get; set; }

    [MaxLength(45)]
    public string? Institucion { get; set; }

    [MaxLength(45)]
    public string? Tipo { get; set; }
}
