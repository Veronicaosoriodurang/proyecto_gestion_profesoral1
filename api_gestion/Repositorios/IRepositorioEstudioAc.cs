using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioEstudioAc
{
    Task<IEnumerable<EstudioAc>> ObtenerTodos(int limite);

    Task<EstudioAc?> ObtenerPorId(
        int estudio,
        string areaConocimiento);

    Task<int> Crear(EstudioAc entidad);

    Task<int> Reemplazar(
        int estudioActual,
        string areaActual,
        EstudioAc entidad);

    Task<int> ActualizarParcial(
        int estudioActual,
        string areaActual,
        EstudioAcCampos campos);

    Task<int> EliminarLogico(
        int estudio,
        string areaConocimiento);
}

public record EstudioAcCampos(
    int? Estudio = null,
    string? AreaConocimiento = null)
{
    public bool HayAlguno =>
        Estudio.HasValue ||
        AreaConocimiento != null;
}
