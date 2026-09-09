using ApiGestion.Modelos;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public interface IServicioTerminoClave
{
    Task<IEnumerable<TerminoClave>> ObtenerTodos(int limite);
    Task<TerminoClave> ObtenerPorTermino(string termino);
    Task Crear(TerminoClave terminoClave);
    Task<int> Reemplazar(string termino, TerminoClave terminoClave);
    Task<int> ActualizarParcial(string termino, TerminoClaveCampos campos);
    Task<int> Eliminar(string termino);
}
