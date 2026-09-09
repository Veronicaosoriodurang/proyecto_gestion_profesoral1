using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioEstudioAc
{
    Task<IEnumerable<EstudioAc>> ObtenerTodos(int limite);

    Task<EstudioAc> ObtenerPorId(
        int estudio,
        string areaConocimiento);

    Task<int> Crear(EstudioAcCrear peticion);

    Task<int> Reemplazar(
        int estudio,
        string areaConocimiento,
        EstudioAcReemplazo peticion);

    Task<int> ActualizarParcial(
        int estudio,
        string areaConocimiento,
        EstudioAcActualizar peticion);

    Task<int> Eliminar(
        int estudio,
        string areaConocimiento);
}
