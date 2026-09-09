namespace ApiGestion.Modelos;

/// <summary>
/// Entidad de dominio que representa la tabla docente_departamento.
/// Tiene llave primaria compuesta por Docente y Departamento.
/// </summary>
public class DocenteDepartamento
{
    /// <summary>FK obligatoria a docente.cedula.</summary>
    public int Docente { get; set; }

    /// <summary>FK obligatoria a programa.id.</summary>
    public int Departamento { get; set; }

    public string Dedicacion { get; set; } = string.Empty;

    public string Modalidad { get; set; } = string.Empty;

    public DateOnly FechaIngreso { get; set; }

    public DateOnly? FechaSalida { get; set; }
}
