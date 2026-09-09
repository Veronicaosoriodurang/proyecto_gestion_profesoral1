using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioExperiecia
{
    Task<IEnumerable<Experiecia>> ObtenerTodos(int limite);
    Task<Experiecia?> ObtenerPorId(int id);
    Task<int> Crear(Experiecia experiencia);
    Task<int> Reemplazar(int id, Experiecia experiencia);
    Task<int> ActualizarParcial(int id, ExperieciaCampos campos);
    Task<int> EliminarLogico(int id);
}

public record ExperieciaCampos(
    string? NombreCargo = null,
    string? Institucion = null,
    string? Tipo = null,
    DateOnly? FechaInicio = null,
    DateOnly? FechaFin = null,
    int? Docente = null)
{
    public bool HayAlguno =>
        NombreCargo != null ||
        Institucion != null ||
        Tipo != null ||
        FechaInicio.HasValue ||
        FechaFin.HasValue ||
        Docente.HasValue;
}
