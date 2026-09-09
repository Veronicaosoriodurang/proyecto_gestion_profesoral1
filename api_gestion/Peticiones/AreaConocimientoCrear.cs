using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class AreaConocimientoCrear
{
    [Required(ErrorMessage = "El campo id es obligatorio.")]
    [MaxLength(6, ErrorMessage = "El campo id no puede exceder los 6 caracteres.")]
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo granArea es obligatorio.")]
    [MaxLength(60, ErrorMessage = "El campo granArea no puede exceder los 60 caracteres.")]
    public string GranArea { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo area es obligatorio.")]
    [MaxLength(60, ErrorMessage = "El campo area no puede exceder los 60 caracteres.")]
    public string Area { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo disciplina es obligatorio.")]
    [MaxLength(150, ErrorMessage = "El campo disciplina no puede exceder los 150 caracteres.")]
    public string Disciplina { get; set; } = string.Empty;
}
