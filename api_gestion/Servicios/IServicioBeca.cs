using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioBeca
{
    Task<IEnumerable<Beca>> ObtenerTodos(int limite);

    Task<Beca> ObtenerPorId(int estudios);

    Task<int> Crear(BecaCrear peticion);

    Task<int> Reemplazar(
        int estudios,
        BecaReemplazo peticion);

    Task<int> ActualizarParcial(
        int estudios,
        BecaActualizar peticion);

    Task<int> Eliminar(int estudios);
}
