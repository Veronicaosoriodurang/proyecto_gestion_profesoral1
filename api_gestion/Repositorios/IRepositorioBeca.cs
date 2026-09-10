using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioBeca
{
    Task<IEnumerable<Beca>> ObtenerTodos(int limite);

    Task<Beca?> ObtenerPorId(int estudios);

    Task<int> Crear(Beca entidad);

    Task<int> Reemplazar(
        int estudiosActual,
        Beca entidad);

    Task<int> ActualizarParcial(
        int estudiosActual,
        BecaCampos campos);

    Task<int> EliminarLogico(int estudios);
}

public record BecaCampos(
    int? Estudios = null,
    string? Tipo = null,
    string? Institucion = null,
    DateOnly? FechaInicio = null,
    DateOnly? FechaFin = null)
{
    public bool HayAlguno =>
        Estudios.HasValue ||
        Tipo != null ||
        Institucion != null ||
        FechaInicio.HasValue ||
        FechaFin.HasValue;
}
