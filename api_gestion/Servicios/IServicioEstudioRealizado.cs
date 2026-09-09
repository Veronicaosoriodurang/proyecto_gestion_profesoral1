using ApiGestion.Modelos;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public interface IServicioEstudioRealizado
{
    Task<IEnumerable<EstudioRealizado>> ObtenerTodos(int limite);
    Task<EstudioRealizado> ObtenerPorId(int id);
    Task Crear(EstudioRealizado estudio);
    Task<int> Reemplazar(int id, EstudioRealizado estudio);
    Task<int> ActualizarParcial(int id, EstudioRealizadoCampos campos);
    Task<int> Eliminar(int id);
}
