using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioInteresFuturo
{
    Task<IEnumerable<InteresFuturo>> ObtenerTodos(int limite);

    Task<InteresFuturo> ObtenerPorId(
        int docente,
        string terminoClave);

    Task Crear(InteresFuturoCrear peticion);

    Task<int> Reemplazar(
        int docenteActual,
        string terminoActual,
        InteresFuturoReemplazo peticion);

    Task<int> ActualizarParcial(
        int docenteActual,
        string terminoActual,
        InteresFuturoActualizar peticion);

    Task<int> Eliminar(
        int docente,
        string terminoClave);
}
