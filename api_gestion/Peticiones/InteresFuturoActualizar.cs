using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class InteresFuturoActualizar
{
    public int? Docente { get; set; }

    [MaxLength(30, ErrorMessage = "El campo terminoClave no puede exceder los 30 caracteres.")]
    public string? TerminoClave { get; set; }
}
