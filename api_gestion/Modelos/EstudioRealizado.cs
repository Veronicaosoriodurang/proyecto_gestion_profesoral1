namespace ApiGestion.Modelos;

/// <summary>
/// Entidad de dominio que representa la tabla estudios_realizados.
/// No incluye Activo: el borrado lógico es un detalle interno del motor.
/// </summary>
public class EstudioRealizado
{
    /// <summary>Llave primaria del estudio. No es Identity.</summary>
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Universidad { get; set; } = string.Empty;

    public DateOnly Fecha { get; set; }

    public string Tipo { get; set; } = string.Empty;

    public string Ciudad { get; set; } = string.Empty;

    /// <summary>FK obligatoria a docente.cedula.</summary>
    public int Docente { get; set; }

    public byte InsAcreditada { get; set; }

    public string Metodologia { get; set; } = string.Empty;

    public string PerfilEgresado { get; set; } = string.Empty;

    public string Pais { get; set; } = string.Empty;
}
