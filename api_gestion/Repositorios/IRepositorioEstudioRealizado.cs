using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

/// <summary>
/// Contrato de la capa de datos para estudios_realizados.
/// </summary>
public interface IRepositorioEstudioRealizado
{
    Task<IEnumerable<EstudioRealizado>> ObtenerTodos(int limite);
    Task<EstudioRealizado?> ObtenerPorId(int id);
    Task Crear(EstudioRealizado estudio);
    Task<int> Reemplazar(EstudioRealizado estudio);
    Task<int> ActualizarParcial(int id, EstudioRealizadoCampos campos);
    Task<int> EliminarLogico(int id);
}

/// <summary>
/// Campos opcionales que puede traer un PATCH.
/// </summary>
public record EstudioRealizadoCampos(
    string? Titulo = null,
    string? Universidad = null,
    DateOnly? Fecha = null,
    string? Tipo = null,
    string? Ciudad = null,
    int? Docente = null,
    byte? InsAcreditada = null,
    string? Metodologia = null,
    string? PerfilEgresado = null,
    string? Pais = null)
{
    public bool HayAlguno =>
        Titulo != null
        || Universidad != null
        || Fecha != null
        || Tipo != null
        || Ciudad != null
        || Docente != null
        || InsAcreditada != null
        || Metodologia != null
        || PerfilEgresado != null
        || Pais != null;
}
