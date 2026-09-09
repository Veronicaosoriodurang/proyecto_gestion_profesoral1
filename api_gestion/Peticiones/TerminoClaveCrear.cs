using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class TerminoClaveCrear
{
    [Required(ErrorMessage = "El campo termino es obligatorio.")]
    [MaxLength(30, ErrorMessage = "El campo termino no puede exceder los 30 caracteres.")]
    public string Termino { get; set; } = string.Empty;

    [MaxLength(30, ErrorMessage = "El campo terminoIngles no puede exceder los 30 caracteres.")]
    public string? TerminoIngles { get; set; }
}
