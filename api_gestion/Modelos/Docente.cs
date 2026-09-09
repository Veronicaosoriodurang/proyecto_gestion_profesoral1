namespace ApiGestion.Modelos;

/// <summary>
/// Entidad de dominio que representa la tabla docente.
/// No incluye Activo: el borrado lógico es un detalle interno del motor.
/// Se conserva el tipográfico del esquema: Nacionalidaad / nacionalidaad.
/// </summary>
public class Docente
{
    /// <summary>Cédula del docente. Es la llave primaria (no Identity).</summary>
    public int Cedula { get; set; }

    public string Nombres { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public string Genero { get; set; } = string.Empty;

    public string Cargo { get; set; } = string.Empty;

    public DateOnly FechaNacimiento { get; set; }

    public string Correo { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string UrlCvlac { get; set; } = string.Empty;

    public DateOnly FechaActualizacion { get; set; }

    public string Escalafon { get; set; } = string.Empty;

    public string Perfil { get; set; } = string.Empty;

    /// <summary>Único string opcional del esquema.</summary>
    public string? CatMinciencia { get; set; }

    public string ConvMinciencia { get; set; } = string.Empty;

    /// <summary>Tipográfico del esquema (nacionalidaad).</summary>
    public string Nacionalidaad { get; set; } = string.Empty;

    /// <summary>FK opcional a linea_investigacion.id.</summary>
    public int? LineaInvestigacionPrincipal { get; set; }
}
