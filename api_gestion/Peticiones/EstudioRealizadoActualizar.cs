namespace ApiGestion.Peticiones;

/// <summary>
/// Cuerpo del PATCH para estudios_realizados.
/// Todos los campos son opcionales.
/// </summary>
public class EstudioRealizadoActualizar
{
    public string? Titulo { get; set; }

    public string? Universidad { get; set; }

    public DateOnly? Fecha { get; set; }

    public string? Tipo { get; set; }

    public string? Ciudad { get; set; }

    public int? Docente { get; set; }

    public byte? InsAcreditada { get; set; }

    public string? Metodologia { get; set; }

    public string? PerfilEgresado { get; set; }

    public string? Pais { get; set; }
}
