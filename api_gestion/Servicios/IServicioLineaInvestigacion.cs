using ApiGestion.Modelos;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public interface IServicioLineaInvestigacion
{
    Task<IEnumerable<LineaInvestigacion>> ObtenerTodos(int limite);
    Task<LineaInvestigacion> ObtenerPorId(int id);
    Task Crear(LineaInvestigacion linea);
    Task<int> Reemplazar(int id, LineaInvestigacion linea);
    Task<int> ActualizarParcial(int id, LineaInvestigacionCampos campos);
    Task<int> Eliminar(int id);
}
