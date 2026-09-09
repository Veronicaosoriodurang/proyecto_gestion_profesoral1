using ApiGestion.Modelos;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public interface IServicioRed
{
    Task<IEnumerable<Red>> ObtenerTodos(int limite);

    Task<Red> ObtenerPorId(int idr);

    Task Crear(Red red);

    Task<int> Reemplazar(int idr, Red red);

    Task<int> ActualizarParcial(int idr, RedCampos campos);

    Task<int> Eliminar(int idr);
}