using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class LineaInvestigacionActualizar
{
    [MaxLength(45, ErrorMessage = "El campo nombre no puede exceder los 45 caracteres.")]
    public string? Nombre { get; set; }

    [MaxLength(256, ErrorMessage = "El campo descripcion no puede exceder los 256 caracteres.")]
    public string? Descripcion { get; set; }
}
