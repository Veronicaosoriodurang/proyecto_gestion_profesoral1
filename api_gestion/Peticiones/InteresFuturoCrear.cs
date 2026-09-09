using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class InteresFuturoCrear
{
    [Required(ErrorMessage = "El campo docente es obligatorio.")]
    public int? Docente { get; set; }

    [Required(ErrorMessage = "El campo terminoClave es obligatorio.")]
    [MaxLength(30, ErrorMessage = "El campo terminoClave no puede exceder los 30 caracteres.")]
    public string TerminoClave { get; set; } = string.Empty;
}
