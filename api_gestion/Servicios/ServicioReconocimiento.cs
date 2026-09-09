using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioReconocimiento : IServicioReconocimiento
{
    private readonly IRepositorioReconocimiento _repositorio;
    private readonly IRepositorioDocente _docentes;

    public ServicioReconocimiento(
        IRepositorioReconocimiento repositorio,
        IRepositorioDocente docentes)
    {
        _repositorio = repositorio;
        _docentes = docentes;
    }

    public async Task<IEnumerable<Reconocimiento>> ObtenerTodos(
        int limite)
    {
        if (limite <= 0)
            throw new ArgumentException(
                "El parámetro limite debe ser mayor a 0.");

        return await _repositorio.ObtenerTodos(limite);
    }

    public async Task<Reconocimiento> ObtenerPorId(int id)
    {
        var dato = await _repositorio.ObtenerPorId(id);

        if (dato == null)
            throw new NoEncontradoExcepcion(
                $"El reconocimiento con id {id} no existe o está inactivo.");

        return dato;
    }

    public async Task<int> Crear(ReconocimientoCrear peticion)
    {
        var docente = peticion.Docente!.Value;

        await ValidarDocente(docente);

        return await _repositorio.Crear(
            new Reconocimiento
            {
                Tipo = peticion.Tipo.Trim(),
                Fecha = peticion.Fecha!.Value,
                Institucion = peticion.Institucion.Trim(),
                Nombre = peticion.Nombre.Trim(),
                Ambito = peticion.Ambito.Trim(),
                Docente = docente
            });
    }

    public async Task<int> Reemplazar(
        int id,
        ReconocimientoReemplazo peticion)
    {
        await ObtenerPorId(id);

        var docente = peticion.Docente!.Value;

        await ValidarDocente(docente);

        var filas = await _repositorio.Reemplazar(
            id,
            new Reconocimiento
            {
                Id = id,
                Tipo = peticion.Tipo.Trim(),
                Fecha = peticion.Fecha!.Value,
                Institucion = peticion.Institucion.Trim(),
                Nombre = peticion.Nombre.Trim(),
                Ambito = peticion.Ambito.Trim(),
                Docente = docente
            });

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                "El reconocimiento no existe o está inactivo.");

        return filas;
    }

    public async Task<int> ActualizarParcial(
        int id,
        ReconocimientoActualizar peticion)
    {
        var campos = new ReconocimientoCampos(
            peticion.Tipo?.Trim(),
            peticion.Fecha,
            peticion.Institucion?.Trim(),
            peticion.Nombre?.Trim(),
            peticion.Ambito?.Trim(),
            peticion.Docente);

        if (!campos.HayAlguno)
            throw new ArgumentException(
                "No se envió ningún campo para actualizar.");

        await ObtenerPorId(id);

        if (peticion.Docente.HasValue)
            await ValidarDocente(peticion.Docente.Value);

        var filas =
            await _repositorio.ActualizarParcial(id, campos);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                "El reconocimiento no existe o está inactivo.");

        return filas;
    }

    public async Task<int> Eliminar(int id)
    {
        var filas = await _repositorio.EliminarLogico(id);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                $"El reconocimiento con id {id} no existe o está inactivo.");

        return filas;
    }

    private async Task ValidarDocente(int cedula)
    {
        if (await _docentes.ObtenerPorId(cedula) == null)
            throw new ArgumentException(
                $"El docente con cédula {cedula} no existe o no está activo.");
    }
}
