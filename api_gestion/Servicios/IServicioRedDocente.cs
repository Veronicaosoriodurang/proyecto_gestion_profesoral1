using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioRedDocente
{
    Task<IEnumerable<RedDocente>> ObtenerTodos(int limite);

    Task<RedDocente> ObtenerPorId(
        int red,
        int docente);

    Task<int> Crear(RedDocenteCrear peticion);

    Task<int> Reemplazar(
        int red,
        int docente,
        RedDocenteReemplazo peticion);

    Task<int> ActualizarParcial(
        int red,
        int docente,
        RedDocenteActualizar peticion);

    Task<int> Eliminar(
        int red,
        int docente);
}
