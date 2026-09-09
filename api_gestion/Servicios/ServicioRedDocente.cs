using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioRedDocente : IServicioRedDocente
{
    private readonly IRepositorioRedDocente _repositorio;
    private readonly IRepositorioRed _redes;
    private readonly IRepositorioDocente _docentes;

    public ServicioRedDocente(
        IRepositorioRedDocente repositorio,
        IRepositorioRed redes,
        IRepositorioDocente docentes)
    {
        _repositorio = repositorio;
        _redes = redes;
        _docentes = docentes;
    }

    public async Task<IEnumerable<RedDocente>> ObtenerTodos(
        int limite)
    {
        if (limite <= 0)
            throw new ArgumentException(
                "El parámetro limite debe ser mayor a 0.");

        return await _repositorio.ObtenerTodos(limite);
    }

    public async Task<RedDocente> ObtenerPorId(
        int red,
        int docente)
    {
        var dato =
            await _repositorio.ObtenerPorId(red, docente);

        if (dato == null)
            throw new NoEncontradoExcepcion(
                "La relación red-docente no existe o está inactiva.");

        return dato;
    }

    public async Task<int> Crear(
        RedDocenteCrear peticion)
    {
        var red = peticion.Red!.Value;
        var docente = peticion.Docente!.Value;

        await ValidarRed(red);
        await ValidarDocente(docente);

        return await _repositorio.Crear(
            new RedDocente
            {
                Red = red,
                Docente = docente,
                FechaInicio = peticion.FechaInicio!.Value,
                FechaFin = peticion.FechaFin?.Trim(),
                ActDestacadas =
                    peticion.ActDestacadas.Trim()
            });
    }

    public async Task<int> Reemplazar(
        int red,
        int docente,
        RedDocenteReemplazo peticion)
    {
        await ObtenerPorId(red, docente);

        var redNueva = peticion.Red!.Value;
        var docenteNuevo = peticion.Docente!.Value;

        await ValidarRed(redNueva);
        await ValidarDocente(docenteNuevo);

        var filas = await _repositorio.Reemplazar(
            red,
            docente,
            new RedDocente
            {
                Red = redNueva,
                Docente = docenteNuevo,
                FechaInicio = peticion.FechaInicio!.Value,
                FechaFin = peticion.FechaFin?.Trim(),
                ActDestacadas =
                    peticion.ActDestacadas.Trim()
            });

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                "La relación red-docente no existe o está inactiva.");

        return filas;
    }

    public async Task<int> ActualizarParcial(
        int red,
        int docente,
        RedDocenteActualizar peticion)
    {
        var campos = new RedDocenteCampos(
            peticion.Red,
            peticion.Docente,
            peticion.FechaInicio,
            peticion.FechaFin?.Trim(),
            peticion.ActDestacadas?.Trim());

        if (!campos.HayAlguno)
            throw new ArgumentException(
                "No se envió ningún campo para actualizar.");

        await ObtenerPorId(red, docente);

        if (peticion.Red.HasValue)
            await ValidarRed(peticion.Red.Value);

        if (peticion.Docente.HasValue)
            await ValidarDocente(peticion.Docente.Value);

        var filas =
            await _repositorio.ActualizarParcial(
                red,
                docente,
                campos);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                "La relación red-docente no existe o está inactiva.");

        return filas;
    }

    public async Task<int> Eliminar(
        int red,
        int docente)
    {
        var filas =
            await _repositorio.EliminarLogico(
                red,
                docente);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                "La relación red-docente no existe o está inactiva.");

        return filas;
    }

    private async Task ValidarRed(int id)
    {
        if (await _redes.ObtenerPorId(id) == null)
            throw new ArgumentException(
                $"La red con id {id} no existe o no está activa.");
    }

    private async Task ValidarDocente(int cedula)
    {
        if (await _docentes.ObtenerPorId(cedula) == null)
            throw new ArgumentException(
                $"El docente con cédula {cedula} no existe o no está activo.");
    }
}
