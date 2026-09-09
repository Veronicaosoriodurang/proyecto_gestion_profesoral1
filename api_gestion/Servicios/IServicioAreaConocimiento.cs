using ApiGestion.Modelos;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public interface IServicioAreaConocimiento
{
    Task<IEnumerable<AreaConocimiento>> ObtenerTodos(int limite);
    Task<AreaConocimiento> ObtenerPorId(string id);
    Task Crear(AreaConocimiento areaConocimiento);
    Task<int> Reemplazar(string id, AreaConocimiento areaConocimiento);
    Task<int> ActualizarParcial(string id, AreaConocimientoCampos campos);
    Task<int> Eliminar(string id);
}
