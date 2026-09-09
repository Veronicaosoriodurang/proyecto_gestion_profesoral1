using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioRed : IServicioRed
{
    private readonly IRepositorioRed _repositorio;

    public ServicioRed(IRepositorioRed repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<Red>> ObtenerTodos(int limite)
    {
        if (limite <= 0)
        {
            throw new ArgumentException("El parámetro limite debe ser un número mayor a 0.");
        }

        return await _repositorio.ObtenerTodos(limite);
    }

    public async Task<Red> ObtenerPorId(int idr)
    {
        var red = await _repositorio.ObtenerPorId(idr);

        if (red == null)
        {
            throw new NoEncontradoExcepcion($"No existe una red con el código {idr}.");
        }

        return red;
    }

    public async Task Crear(Red red)
    {
        await _repositorio.Crear(red);
    }

    public async Task<int> Reemplazar(int idr, Red red)
    {
        red.Idr = idr;

        var filas = await _repositorio.Reemplazar(red);

        if (filas == 0)
        {
            throw new NoEncontradoExcepcion($"No existe una red con el código {idr}.");
        }

        return filas;
    }

    public async Task<int> ActualizarParcial(int idr, RedCampos campos)
    {
        if (!campos.HayAlguno)
        {
            throw new ArgumentException("No se envió ningún campo para actualizar.");
        }

        var filas = await _repositorio.ActualizarParcial(idr, campos);

        if (filas == 0)
        {
            throw new NoEncontradoExcepcion($"No existe una red con el código {idr}.");
        }

        return filas;
    }

    public async Task<int> Eliminar(int idr)
    {
        var filas = await _repositorio.EliminarLogico(idr);

        if (filas == 0)
        {
            throw new NoEncontradoExcepcion($"No existe una red con el código {idr}.");
        }

        return filas;
    }
}
