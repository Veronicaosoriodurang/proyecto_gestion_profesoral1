using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class EvaluacionDocenteActualizar
{
    public float? Calificacion { get; set; }

    [MaxLength(45, ErrorMessage = "El semestre no puede exceder los 45 caracteres.")]
    public string? Semestre { get; set; }

    public int? Docente { get; set; }
}
