using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioReconocimiento
{
    Task<IEnumerable<Reconocimiento>> ObtenerTodos(int limite);
    Task<Reconocimiento?> ObtenerPorId(int id);
    Task<int> Crear(Reconocimiento reconocimiento);
    Task<int> Reemplazar(int id, Reconocimiento reconocimiento);
    Task<int> ActualizarParcial(int id, ReconocimientoCampos campos);
    Task<int> EliminarLogico(int id);
}

public record ReconocimientoCampos(
    string? Tipo = null,
    DateOnly? Fecha = null,
    string? Institucion = null,
    string? Nombre = null,
    string? Ambito = null,
    int? Docente = null)
{
    public bool HayAlguno =>
        Tipo != null ||
        Fecha.HasValue ||
        Institucion != null ||
        Nombre != null ||
        Ambito != null ||
        Docente.HasValue;
}
