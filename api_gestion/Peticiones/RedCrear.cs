using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class RedCrear
{
    [Required(ErrorMessage = "El campo idr es obligatorio.")]
    public int? Idr { get; set; }

    [Required(ErrorMessage = "El campo nombre es obligatorio.")]
    [MaxLength(45, ErrorMessage = "El campo nombre no puede exceder los 45 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo url es obligatorio.")]
    [MaxLength(45, ErrorMessage = "El campo url no puede exceder los 45 caracteres.")]
    public string Url { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo pais es obligatorio.")]
    [MaxLength(45, ErrorMessage = "El campo pais no puede exceder los 45 caracteres.")]
    public string Pais { get; set; } = string.Empty;
}