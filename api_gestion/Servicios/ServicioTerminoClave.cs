using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioTerminoClave : IServicioTerminoClave
{
    private readonly IRepositorioTerminoClave _repositorio;

    public ServicioTerminoClave(IRepositorioTerminoClave repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<TerminoClave>> ObtenerTodos(int limite)
    {
        if (limite <= 0)
            throw new ArgumentException("El límite debe ser mayor que cero.");

        return await _repositorio.ObtenerTodos(limite);
    }

    public async Task<TerminoClave> ObtenerPorTermino(string termino)
    {
        var entidad = await _repositorio.ObtenerPorTermino(termino);

        if (entidad == null)
            throw new NoEncontradoExcepcion(
                $"No existe un término clave con el valor '{termino}'.");

        return entidad;
    }

    public async Task Crear(TerminoClave terminoClave)
    {
        await _repositorio.Crear(terminoClave);
    }

    public async Task<int> Reemplazar(
        string termino,
        TerminoClave terminoClave)
    {
        terminoClave.Termino = termino;

        var filas = await _repositorio.Reemplazar(terminoClave);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                $"No existe un término clave con el valor '{termino}'.");

        return filas;
    }

    public async Task<int> ActualizarParcial(
        string termino,
        TerminoClaveCampos campos)
    {
        if (!campos.HayAlguno)
            throw new ArgumentException(
                "Debe enviar al menos un campo para actualizar.");

        var filas = await _repositorio.ActualizarParcial(termino, campos);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                $"No existe un término clave con el valor '{termino}'.");

        return filas;
    }

    public async Task<int> Eliminar(string termino)
    {
        var filas = await _repositorio.EliminarLogico(termino);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                $"No existe un término clave con el valor '{termino}'.");

        return filas;
    }
}
