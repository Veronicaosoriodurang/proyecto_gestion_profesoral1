using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioAreaConocimiento : IServicioAreaConocimiento
{
    private readonly IRepositorioAreaConocimiento _repositorio;

    public ServicioAreaConocimiento(IRepositorioAreaConocimiento repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<AreaConocimiento>> ObtenerTodos(int limite)
    {
        if (limite <= 0)
            throw new ArgumentException("El límite debe ser mayor que cero.");

        return await _repositorio.ObtenerTodos(limite);
    }

    public async Task<AreaConocimiento> ObtenerPorId(string id)
    {
        var area = await _repositorio.ObtenerPorId(id);

        if (area == null)
            throw new NoEncontradoExcepcion(
                $"No existe un área de conocimiento con el código {id}.");

        return area;
    }

    public async Task Crear(AreaConocimiento areaConocimiento)
    {
        await _repositorio.Crear(areaConocimiento);
    }

    public async Task<int> Reemplazar(
        string id,
        AreaConocimiento areaConocimiento)
    {
        areaConocimiento.Id = id;

        var filas = await _repositorio.Reemplazar(areaConocimiento);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                $"No existe un área de conocimiento con el código {id}.");

        return filas;
    }

    public async Task<int> ActualizarParcial(
        string id,
        AreaConocimientoCampos campos)
    {
        if (!campos.HayAlguno)
            throw new ArgumentException(
                "Debe enviar al menos un campo para actualizar.");

        var filas = await _repositorio.ActualizarParcial(id, campos);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                $"No existe un área de conocimiento con el código {id}.");

        return filas;
    }

    public async Task<int> Eliminar(string id)
    {
        var filas = await _repositorio.EliminarLogico(id);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                $"No existe un área de conocimiento con el código {id}.");

        return filas;
    }
}
