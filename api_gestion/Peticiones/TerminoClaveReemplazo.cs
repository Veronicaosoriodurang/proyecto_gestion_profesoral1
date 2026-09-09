using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class TerminoClaveReemplazo
{
    [MaxLength(30, ErrorMessage = "El campo terminoIngles no puede exceder los 30 caracteres.")]
    public string? TerminoIngles { get; set; }
}
