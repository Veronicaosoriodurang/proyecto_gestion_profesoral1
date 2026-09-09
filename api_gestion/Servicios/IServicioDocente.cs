using ApiGestion.Modelos;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

/// <summary>
/// Contrato de la capa de negocio para docente.
///   ArgumentException      → 400
///   NoEncontradoExcepcion  → 404
/// </summary>
public interface IServicioDocente
{
    Task<IEnumerable<Docente>> ObtenerTodos(int limite);
    Task<Docente> ObtenerPorId(int cedula);
    Task Crear(Docente docente);
    Task<int> Reemplazar(int cedula, Docente docente);
    Task<int> ActualizarParcial(int cedula, DocenteCampos campos);
    Task<int> Eliminar(int cedula);
}
