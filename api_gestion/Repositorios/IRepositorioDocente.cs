using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

/// <summary>
/// Contrato de la capa de datos para docente.
/// </summary>
public interface IRepositorioDocente
{
    Task<IEnumerable<Docente>> ObtenerTodos(int limite);
    Task<Docente?> ObtenerPorId(int cedula);
    Task Crear(Docente docente);
    Task<int> Reemplazar(Docente docente);
    Task<int> ActualizarParcial(int cedula, DocenteCampos campos);
    Task<int> EliminarLogico(int cedula);
}

/// <summary>
/// Campos que un PATCH puede traer, todos opcionales.
/// </summary>
public record DocenteCampos(
    string? Nombres = null,
    string? Apellidos = null,
    string? Genero = null,
    string? Cargo = null,
    DateOnly? FechaNacimiento = null,
    string? Correo = null,
    string? Telefono = null,
    string? UrlCvlac = null,
    DateOnly? FechaActualizacion = null,
    string? Escalafon = null,
    string? Perfil = null,
    string? CatMinciencia = null,
    string? ConvMinciencia = null,
    string? Nacionalidaad = null,
    int? LineaInvestigacionPrincipal = null)
{
    /// <summary>¿Llegó algún campo? Si no, el PATCH es un 400 y no un 404.</summary>
    public bool HayAlguno =>
        Nombres != null || Apellidos != null || Genero != null || Cargo != null
        || FechaNacimiento != null || Correo != null || Telefono != null
        || UrlCvlac != null || FechaActualizacion != null || Escalafon != null
        || Perfil != null || CatMinciencia != null || ConvMinciencia != null
        || Nacionalidaad != null || LineaInvestigacionPrincipal != null;
}
