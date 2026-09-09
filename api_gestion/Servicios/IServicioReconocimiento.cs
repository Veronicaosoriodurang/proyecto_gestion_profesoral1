using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioReconocimiento
{
    Task<IEnumerable<Reconocimiento>> ObtenerTodos(int limite);
    Task<Reconocimiento> ObtenerPorId(int id);
    Task<int> Crear(ReconocimientoCrear peticion);
    Task<int> Reemplazar(int id, ReconocimientoReemplazo peticion);
    Task<int> ActualizarParcial(int id, ReconocimientoActualizar peticion);
    Task<int> Eliminar(int id);
}
