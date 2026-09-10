using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioApoyoProfesoral
{
    Task<IEnumerable<ApoyoProfesoral>> ObtenerTodos(int limite);

    Task<ApoyoProfesoral> ObtenerPorId(int estudios);

    Task<int> Crear(ApoyoProfesoralCrear peticion);

    Task<int> Reemplazar(
        int estudios,
        ApoyoProfesoralReemplazo peticion);

    Task<int> ActualizarParcial(
        int estudios,
        ApoyoProfesoralActualizar peticion);

    Task<int> Eliminar(int estudios);
}
