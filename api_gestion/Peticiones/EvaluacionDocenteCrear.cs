using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class EvaluacionDocenteCrear
{
    [Required(ErrorMessage = "El campo calificacion es obligatorio.")]
    public float? Calificacion { get; set; }

    [Required(ErrorMessage = "El campo semestre es obligatorio.")]
    [MaxLength(45, ErrorMessage = "El semestre no puede exceder los 45 caracteres.")]
    public string Semestre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo docente es obligatorio.")]
    public int? Docente { get; set; }
}
