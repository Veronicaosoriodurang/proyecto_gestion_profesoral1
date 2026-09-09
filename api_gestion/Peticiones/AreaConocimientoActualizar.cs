using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class AreaConocimientoActualizar
{
    [MaxLength(60, ErrorMessage = "El campo granArea no puede exceder los 60 caracteres.")]
    public string? GranArea { get; set; }

    [MaxLength(60, ErrorMessage = "El campo area no puede exceder los 60 caracteres.")]
    public string? Area { get; set; }

    [MaxLength(150, ErrorMessage = "El campo disciplina no puede exceder los 150 caracteres.")]
    public string? Disciplina { get; set; }
}
