using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

/// <summary>
/// Cuerpo del PUT para docente_departamento.
/// Docente y departamento viajan en la ruta.
/// </summary>
public class DocenteDepartamentoReemplazo
{
    [Required(ErrorMessage = "El campo dedicacion es obligatorio.")]
    [MaxLength(15, ErrorMessage = "El campo dedicacion no puede exceder los 15 caracteres.")]
    public string Dedicacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo modalidad es obligatorio.")]
    [MaxLength(45, ErrorMessage = "El campo modalidad no puede exceder los 45 caracteres.")]
    public string Modalidad { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo fechaIngreso es obligatorio.")]
    public DateOnly? FechaIngreso { get; set; }

    public DateOnly? FechaSalida { get; set; }
}
