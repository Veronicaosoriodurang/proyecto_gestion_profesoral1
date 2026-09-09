using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioLineaInvestigacion
{
    Task<IEnumerable<LineaInvestigacion>> ObtenerTodos(int limite);
    Task<LineaInvestigacion?> ObtenerPorId(int id);
    Task Crear(LineaInvestigacion linea);
    Task<int> Reemplazar(LineaInvestigacion linea);
    Task<int> ActualizarParcial(int id, LineaInvestigacionCampos campos);
    Task<int> EliminarLogico(int id);
}

public record LineaInvestigacionCampos(
    string? Nombre,
    string? Descripcion)
{
    public bool HayAlguno =>
        Nombre != null ||
        Descripcion != null;
}
