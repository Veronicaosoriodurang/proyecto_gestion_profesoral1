using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioEvaluacionDocente
{
    Task<IEnumerable<EvaluacionDocente>> ObtenerTodos(int limite);
    Task<EvaluacionDocente> ObtenerPorId(int id);
    Task<int> Crear(EvaluacionDocenteCrear peticion);

    Task<int> Reemplazar(
        int id,
        EvaluacionDocenteReemplazo peticion);

    Task<int> ActualizarParcial(
        int id,
        EvaluacionDocenteActualizar peticion);

    Task<int> Eliminar(int id);
}
