using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioExperiecia
{
    Task<IEnumerable<Experiecia>> ObtenerTodos(int limite);
    Task<Experiecia> ObtenerPorId(int id);
    Task<int> Crear(ExperieciaCrear peticion);
    Task<int> Reemplazar(int id, ExperieciaReemplazo peticion);
    Task<int> ActualizarParcial(int id, ExperieciaActualizar peticion);
    Task<int> Eliminar(int id);
}
