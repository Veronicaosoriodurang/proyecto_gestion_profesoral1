using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioBeca : IServicioBeca
{
    private readonly IRepositorioBeca _repositorio;
    private readonly IRepositorioEstudioRealizado _estudios;

    public ServicioBeca(
        IRepositorioBeca repositorio,
        IRepositorioEstudioRealizado estudios)
    {
        _repositorio = repositorio;
        _estudios = estudios;
    }

    public async Task<IEnumerable<Beca>> ObtenerTodos(
        int limite)
    {
        if (limite <= 0)
            throw new ArgumentException(
                "El parámetro limite debe ser mayor a 0.");

        return await _repositorio.ObtenerTodos(limite);
    }

    public async Task<Beca> ObtenerPorId(
        int estudios)
    {
        var dato =
            await _repositorio.ObtenerPorId(estudios);

        if (dato == null)
            throw new NoEncontradoExcepcion(
                "La beca no existe o está inactiva.");

        return dato;
    }

    public async Task<int> Crear(
        BecaCrear peticion)
    {
        var estudio = peticion.Estudios!.Value;

        await ValidarEstudio(estudio);

        ValidarFechas(
            peticion.FechaInicio!.Value,
            peticion.FechaFin);

        return await _repositorio.Crear(
            new Beca
            {
                Estudios = estudio,
                Tipo = peticion.Tipo.Trim(),
                Institucion = peticion.Institucion.Trim(),
                FechaInicio = peticion.FechaInicio.Value,
                FechaFin = peticion.FechaFin
            });
    }

    public async Task<int> Reemplazar(
        int estudios,
        BecaReemplazo peticion)
    {
        await ObtenerPorId(estudios);

        var estudioNuevo =
            peticion.Estudios!.Value;

        await ValidarEstudio(estudioNuevo);

        ValidarFechas(
            peticion.FechaInicio!.Value,
            peticion.FechaFin);

        var filas =
            await _repositorio.Reemplazar(
                estudios,
                new Beca
                {
                    Estudios = estudioNuevo,
                    Tipo = peticion.Tipo.Trim(),
                    Institucion = peticion.Institucion.Trim(),
                    FechaInicio = peticion.FechaInicio.Value,
                    FechaFin = peticion.FechaFin
                });

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                "La beca no existe o está inactiva.");

        return filas;
    }

    public async Task<int> ActualizarParcial(
        int estudios,
        BecaActualizar peticion)
    {
        var campos = new BecaCampos(
            peticion.Estudios,
            peticion.Tipo?.Trim(),
            peticion.Institucion?.Trim(),
            peticion.FechaInicio,
            peticion.FechaFin);

        if (!campos.HayAlguno)
            throw new ArgumentException(
                "No se envió ningún campo para actualizar.");

        var actual =
            await ObtenerPorId(estudios);

        if (peticion.Estudios.HasValue)
            await ValidarEstudio(peticion.Estudios.Value);

        var inicio =
            peticion.FechaInicio ??
            actual.FechaInicio;

        var fin =
            peticion.FechaFin ??
            actual.FechaFin;

        ValidarFechas(inicio, fin);

        var filas =
            await _repositorio.ActualizarParcial(
                estudios,
                campos);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                "La beca no existe o está inactiva.");

        return filas;
    }

    public async Task<int> Eliminar(
        int estudios)
    {
        var filas =
            await _repositorio.EliminarLogico(estudios);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                "La beca no existe o está inactiva.");

        return filas;
    }

    private async Task ValidarEstudio(int id)
    {
        if (await _estudios.ObtenerPorId(id) == null)
            throw new ArgumentException(
                $"El estudio con id {id} no existe o no está activo.");
    }

    private static void ValidarFechas(
        DateOnly inicio,
        DateOnly? fin)
    {
        if (fin.HasValue && fin.Value < inicio)
            throw new ArgumentException(
                "La fecha de fin no puede ser anterior a la fecha de inicio.");
    }
}
