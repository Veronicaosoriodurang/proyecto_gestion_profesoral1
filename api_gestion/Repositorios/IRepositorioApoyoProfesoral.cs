using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioApoyoProfesoral
{
    Task<IEnumerable<ApoyoProfesoral>> ObtenerTodos(int limite);

    Task<ApoyoProfesoral?> ObtenerPorId(int estudios);

    Task<int> Crear(ApoyoProfesoral entidad);

    Task<int> Reemplazar(
        int estudiosActual,
        ApoyoProfesoral entidad);

    Task<int> ActualizarParcial(
        int estudiosActual,
        ApoyoProfesoralCampos campos);

    Task<int> EliminarLogico(int estudios);
}

public record ApoyoProfesoralCampos(
    int? Estudios = null,
    byte? ConApoyo = null,
    string? Institucion = null,
    string? Tipo = null)
{
    public bool HayAlguno =>
        Estudios.HasValue ||
        ConApoyo.HasValue ||
        Institucion != null ||
        Tipo != null;
}
