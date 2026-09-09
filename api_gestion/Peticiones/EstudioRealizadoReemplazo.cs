using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

/// <summary>
/// Cuerpo del PUT para estudios_realizados.
/// El id viaja en la ruta, no en el cuerpo.
/// </summary>
public class EstudioRealizadoReemplazo
{
    [Required(ErrorMessage = "El campo titulo es obligatorio.")]
    [MaxLength(45, ErrorMessage = "El campo titulo no puede exceder los 45 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo universidad es obligatorio.")]
    [MaxLength(50, ErrorMessage = "El campo universidad no puede exceder los 50 caracteres.")]
    public string Universidad { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo fecha es obligatorio.")]
    public DateOnly? Fecha { get; set; }

    [Required(ErrorMessage = "El campo tipo es obligatorio.")]
    [MaxLength(45, ErrorMessage = "El campo tipo no puede exceder los 45 caracteres.")]
    public string Tipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo ciudad es obligatorio.")]
    [MaxLength(45, ErrorMessage = "El campo ciudad no puede exceder los 45 caracteres.")]
    public string Ciudad { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo docente es obligatorio.")]
    public int? Docente { get; set; }

    [Required(ErrorMessage = "El campo insAcreditada es obligatorio.")]
    public byte? InsAcreditada { get; set; }

    [Required(ErrorMessage = "El campo metodologia es obligatorio.")]
    [MaxLength(45, ErrorMessage = "El campo metodologia no puede exceder los 45 caracteres.")]
    public string Metodologia { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo perfilEgresado es obligatorio.")]
    public string PerfilEgresado { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo pais es obligatorio.")]
    [MaxLength(45, ErrorMessage = "El campo pais no puede exceder los 45 caracteres.")]
    public string Pais { get; set; } = string.Empty;
}
