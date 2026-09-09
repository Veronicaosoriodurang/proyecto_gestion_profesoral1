using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioDocenteDepartamento
{
    Task<IEnumerable<DocenteDepartamento>> ObtenerTodos(int limite);
    Task<DocenteDepartamento> ObtenerPorId(int docente, int departamento);
    Task Crear(DocenteDepartamentoCrear peticion);
    Task<int> Reemplazar(
        int docente,
        int departamento,
        DocenteDepartamentoReemplazo peticion);
    Task<int> ActualizarParcial(
        int docente,
        int departamento,
        DocenteDepartamentoActualizar peticion);
    Task<int> Eliminar(int docente, int departamento);
}
