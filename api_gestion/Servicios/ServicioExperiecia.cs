using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioExperiecia : IServicioExperiecia
{
    private readonly IRepositorioExperiecia _repositorio;
    private readonly IRepositorioDocente _docentes;

    public ServicioExperiecia(
        IRepositorioExperiecia repositorio,
        IRepositorioDocente docentes)
    {
        _repositorio = repositorio;
        _docentes = docentes;
    }

    public async Task<IEnumerable<Experiecia>> ObtenerTodos(
        int limite)
    {
        if (limite <= 0)
            throw new ArgumentException(
                "El parámetro limite debe ser mayor a 0.");

        return await _repositorio.ObtenerTodos(limite);
    }

    public async Task<Experiecia> ObtenerPorId(int id)
    {
        var dato = await _repositorio.ObtenerPorId(id);

        if (dato == null)
            throw new NoEncontradoExcepcion(
                $"La experiencia con id {id} no existe o está inactiva.");

        return dato;
    }

    public async Task<int> Crear(ExperieciaCrear peticion)
    {
        var docente = peticion.Docente!.Value;

        await ValidarDocente(docente);
        ValidarFechas(
            peticion.FechaInicio!.Value,
            peticion.FechaFin);

        return await _repositorio.Crear(
            new Experiecia
            {
                NombreCargo = peticion.NombreCargo.Trim(),
                Institucion = peticion.Institucion.Trim(),
                Tipo = peticion.Tipo.Trim(),
                FechaInicio = peticion.FechaInicio.Value,
                FechaFin = peticion.FechaFin,
                Docente = docente
            });
    }

    public async Task<int> Reemplazar(
        int id,
        ExperieciaReemplazo peticion)
    {
        await ObtenerPorId(id);

        var docente = peticion.Docente!.Value;

        await ValidarDocente(docente);
        ValidarFechas(
            peticion.FechaInicio!.Value,
            peticion.FechaFin);

        var filas = await _repositorio.Reemplazar(
            id,
            new Experiecia
            {
                Id = id,
                NombreCargo = peticion.NombreCargo.Trim(),
                Institucion = peticion.Institucion.Trim(),
                Tipo = peticion.Tipo.Trim(),
                FechaInicio = peticion.FechaInicio.Value,
                FechaFin = peticion.FechaFin,
                Docente = docente
            });

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                "La experiencia no existe o está inactiva.");

        return filas;
    }

    public async Task<int> ActualizarParcial(
        int id,
        ExperieciaActualizar peticion)
    {
        var campos = new ExperieciaCampos(
            peticion.NombreCargo?.Trim(),
            peticion.Institucion?.Trim(),
            peticion.Tipo?.Trim(),
            peticion.FechaInicio,
            peticion.FechaFin,
            peticion.Docente);

        if (!campos.HayAlguno)
            throw new ArgumentException(
                "No se envió ningún campo para actualizar.");

        var actual = await ObtenerPorId(id);

        if (peticion.Docente.HasValue)
            await ValidarDocente(peticion.Docente.Value);

        var inicio =
            peticion.FechaInicio ?? actual.FechaInicio;

        var fin =
            peticion.FechaFin ?? actual.FechaFin;

        ValidarFechas(inicio, fin);

        var filas =
            await _repositorio.ActualizarParcial(id, campos);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                "La experiencia no existe o está inactiva.");

        return filas;
    }

    public async Task<int> Eliminar(int id)
    {
        var filas = await _repositorio.EliminarLogico(id);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                $"La experiencia con id {id} no existe o está inactiva.");

        return filas;
    }

    private async Task ValidarDocente(int cedula)
    {
        if (await _docentes.ObtenerPorId(cedula) == null)
            throw new ArgumentException(
                $"El docente con cédula {cedula} no existe o no está activo.");
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
