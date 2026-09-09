using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioEvaluacionDocente
{
    Task<IEnumerable<EvaluacionDocente>> ObtenerTodos(int limite);
    Task<EvaluacionDocente?> ObtenerPorId(int id);
    Task<int> Crear(EvaluacionDocente evaluacion);
    Task<int> Reemplazar(int id, EvaluacionDocente evaluacion);
    Task<int> ActualizarParcial(int id, EvaluacionDocenteCampos campos);
    Task<int> EliminarLogico(int id);
}

public record EvaluacionDocenteCampos(
    float? Calificacion = null,
    string? Semestre = null,
    int? Docente = null)
{
    public bool HayAlguno =>
        Calificacion.HasValue ||
        Semestre != null ||
        Docente.HasValue;
}
