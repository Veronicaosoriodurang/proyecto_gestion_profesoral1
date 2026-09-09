namespace ApiGestion.Peticiones;

/// <summary>
/// Cuerpo del PATCH para red.
/// Todos los campos son opcionales y solo se actualizan los que lleguen.
/// </summary>
public class RedActualizar
{
    public string? Nombre { get; set; }

    public string? Url { get; set; }

    public string? Pais { get; set; }
}