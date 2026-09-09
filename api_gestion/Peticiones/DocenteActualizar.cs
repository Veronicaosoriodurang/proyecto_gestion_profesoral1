namespace ApiGestion.Peticiones;

/// <summary>
/// Cuerpo del PATCH: todos los campos opcionales; solo se escriben los que lleguen.
/// </summary>
public class DocenteActualizar
{
    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public string? Genero { get; set; }
    public string? Cargo { get; set; }
    public DateOnly? FechaNacimiento { get; set; }
    public string? Correo { get; set; }
    public string? Telefono { get; set; }
    public string? UrlCvlac { get; set; }
    public DateOnly? FechaActualizacion { get; set; }
    public string? Escalafon { get; set; }
    public string? Perfil { get; set; }
    public string? CatMinciencia { get; set; }
    public string? ConvMinciencia { get; set; }
    public string? Nacionalidaad { get; set; }
    public int? LineaInvestigacionPrincipal { get; set; }
}
