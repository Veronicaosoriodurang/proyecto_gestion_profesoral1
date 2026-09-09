using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

/// <summary>
/// Cuerpo del PATCH para docente_departamento.
/// Todos los campos son opcionales.
/// </summary>
public class DocenteDepartamentoActualizar
{
    [MaxLength(15, ErrorMessage = "El campo dedicacion no puede exceder los 15 caracteres.")]
    public string? Dedicacion { get; set; }

    [MaxLength(45, ErrorMessage = "El campo modalidad no puede exceder los 45 caracteres.")]
    public string? Modalidad { get; set; }

    public DateOnly? FechaIngreso { get; set; }

    public DateOnly? FechaSalida { get; set; }
}
