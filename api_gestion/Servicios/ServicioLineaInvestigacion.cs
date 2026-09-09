using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioLineaInvestigacion : IServicioLineaInvestigacion
{
    private readonly IRepositorioLineaInvestigacion _repositorio;

    public ServicioLineaInvestigacion(IRepositorioLineaInvestigacion repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<LineaInvestigacion>> ObtenerTodos(int limite)
    {
        if (limite <= 0)
            throw new ArgumentException("El límite debe ser mayor que cero.");

        return await _repositorio.ObtenerTodos(limite);
    }

    public async Task<LineaInvestigacion> ObtenerPorId(int id)
    {
        var linea = await _repositorio.ObtenerPorId(id);

        if (linea == null)
            throw new NoEncontradoExcepcion(
                $"No existe una línea de investigación con el código {id}.");

        return linea;
    }

    public async Task Crear(LineaInvestigacion linea)
    {
        await _repositorio.Crear(linea);
    }

    public async Task<int> Reemplazar(int id, LineaInvestigacion linea)
    {
        linea.Id = id;

        var filas = await _repositorio.Reemplazar(linea);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                $"No existe una línea de investigación con el código {id}.");

        return filas;
    }

    public async Task<int> ActualizarParcial(
        int id,
        LineaInvestigacionCampos campos)
    {
        if (!campos.HayAlguno)
            throw new ArgumentException(
                "Debe enviar al menos un campo para actualizar.");

        var filas = await _repositorio.ActualizarParcial(id, campos);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                $"No existe una línea de investigación con el código {id}.");

        return filas;
    }

    public async Task<int> Eliminar(int id)
    {
        var filas = await _repositorio.EliminarLogico(id);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                $"No existe una línea de investigación con el código {id}.");

        return filas;
    }
}
