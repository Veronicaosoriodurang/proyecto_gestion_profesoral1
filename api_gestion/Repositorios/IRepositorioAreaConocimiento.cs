using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioAreaConocimiento
{
    Task<IEnumerable<AreaConocimiento>> ObtenerTodos(int limite);
    Task<AreaConocimiento?> ObtenerPorId(string id);
    Task Crear(AreaConocimiento areaConocimiento);
    Task<int> Reemplazar(AreaConocimiento areaConocimiento);
    Task<int> ActualizarParcial(string id, AreaConocimientoCampos campos);
    Task<int> EliminarLogico(string id);
}

public record AreaConocimientoCampos(
    string? GranArea,
    string? Area,
    string? Disciplina)
{
    public bool HayAlguno =>
        GranArea != null ||
        Area != null ||
        Disciplina != null;
}
