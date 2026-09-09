using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

/// <summary>
/// Capa de negocio para estudios_realizados.
/// Valida que el docente relacionado exista y esté activo.
/// </summary>
public class ServicioEstudioRealizado : IServicioEstudioRealizado
{
    private readonly IRepositorioEstudioRealizado _repositorio;
    private readonly IRepositorioDocente _docentes;

    public ServicioEstudioRealizado(
        IRepositorioEstudioRealizado repositorio,
        IRepositorioDocente docentes)
    {
        _repositorio = repositorio;
        _docentes = docentes;
    }

    public async Task<IEnumerable<EstudioRealizado>> ObtenerTodos(int limite)
    {
        if (limite <= 0)
        {
            throw new ArgumentException(
                "El parámetro limite debe ser un número mayor a 0.");
        }

        return await _repositorio.ObtenerTodos(limite);
    }

    public async Task<EstudioRealizado> ObtenerPorId(int id)
    {
        var estudio = await _repositorio.ObtenerPorId(id);

        if (estudio == null)
        {
            throw new NoEncontradoExcepcion(
                $"No existe un estudio realizado con el id {id}.");
        }

        return estudio;
    }

    public async Task Crear(EstudioRealizado estudio)
    {
        await ValidarDocente(estudio.Docente);
        await _repositorio.Crear(estudio);
    }

    public async Task<int> Reemplazar(
        int id,
        EstudioRealizado estudio)
    {
        estudio.Id = id;

        await ValidarDocente(estudio.Docente);

        var filasAfectadas =
            await _repositorio.Reemplazar(estudio);

        if (filasAfectadas == 0)
        {
            throw new NoEncontradoExcepcion(
                $"No existe un estudio realizado con el id {id}.");
        }

        return filasAfectadas;
    }

    public async Task<int> ActualizarParcial(
        int id,
        EstudioRealizadoCampos campos)
    {
        if (!campos.HayAlguno)
        {
            throw new ArgumentException(
                "No se envió ningún campo para actualizar.");
        }

        if (campos.Docente.HasValue)
        {
            await ValidarDocente(campos.Docente.Value);
        }

        var filasAfectadas =
            await _repositorio.ActualizarParcial(id, campos);

        if (filasAfectadas == 0)
        {
            throw new NoEncontradoExcepcion(
                $"No existe un estudio realizado con el id {id}.");
        }

        return filasAfectadas;
    }

    public async Task<int> Eliminar(int id)
    {
        var filasAfectadas =
            await _repositorio.EliminarLogico(id);

        if (filasAfectadas == 0)
        {
            throw new NoEncontradoExcepcion(
                $"No existe un estudio realizado con el id {id}.");
        }

        return filasAfectadas;
    }

    private async Task ValidarDocente(int cedula)
    {
        var docente = await _docentes.ObtenerPorId(cedula);

        if (docente == null)
        {
            throw new ArgumentException(
                $"El docente con cédula {cedula} no existe o no está activo.");
        }
    }
}
