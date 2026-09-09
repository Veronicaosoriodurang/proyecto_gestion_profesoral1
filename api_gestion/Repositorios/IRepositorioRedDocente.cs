using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioRedDocente
{
    Task<IEnumerable<RedDocente>> ObtenerTodos(int limite);

    Task<RedDocente?> ObtenerPorId(
        int red,
        int docente);

    Task<int> Crear(RedDocente entidad);

    Task<int> Reemplazar(
        int redActual,
        int docenteActual,
        RedDocente entidad);

    Task<int> ActualizarParcial(
        int redActual,
        int docenteActual,
        RedDocenteCampos campos);

    Task<int> EliminarLogico(
        int red,
        int docente);
}

public record RedDocenteCampos(
    int? Red = null,
    int? Docente = null,
    DateOnly? FechaInicio = null,
    string? FechaFin = null,
    string? ActDestacadas = null)
{
    public bool HayAlguno =>
        Red.HasValue ||
        Docente.HasValue ||
        FechaInicio.HasValue ||
        FechaFin != null ||
        ActDestacadas != null;
}
