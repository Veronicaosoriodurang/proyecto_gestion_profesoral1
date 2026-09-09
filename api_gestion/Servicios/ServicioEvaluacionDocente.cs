using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioEvaluacionDocente
    : IServicioEvaluacionDocente
{
    private readonly IRepositorioEvaluacionDocente _repositorio;
    private readonly IRepositorioDocente _docentes;

    public ServicioEvaluacionDocente(
        IRepositorioEvaluacionDocente repositorio,
        IRepositorioDocente docentes)
    {
        _repositorio = repositorio;
        _docentes = docentes;
    }

    public async Task<IEnumerable<EvaluacionDocente>> ObtenerTodos(
        int limite)
    {
        if (limite <= 0)
            throw new ArgumentException(
                "El parámetro limite debe ser un número mayor a 0.");

        return await _repositorio.ObtenerTodos(limite);
    }

    public async Task<EvaluacionDocente> ObtenerPorId(int id)
    {
        var evaluacion = await _repositorio.ObtenerPorId(id);

        if (evaluacion == null)
            throw new NoEncontradoExcepcion(
                $"La evaluación con id {id} no existe o está inactiva.");

        return evaluacion;
    }

    public async Task<int> Crear(
        EvaluacionDocenteCrear peticion)
    {
        var docente = peticion.Docente!.Value;

        await ValidarDocente(docente);

        return await _repositorio.Crear(
            new EvaluacionDocente
            {
                Calificacion = peticion.Calificacion!.Value,
                Semestre = peticion.Semestre.Trim(),
                Docente = docente
            });
    }

    public async Task<int> Reemplazar(
        int id,
        EvaluacionDocenteReemplazo peticion)
    {
        await ObtenerPorId(id);

        var docente = peticion.Docente!.Value;

        await ValidarDocente(docente);

        var filas = await _repositorio.Reemplazar(
            id,
            new EvaluacionDocente
            {
                Id = id,
                Calificacion = peticion.Calificacion!.Value,
                Semestre = peticion.Semestre.Trim(),
                Docente = docente
            });

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                $"La evaluación con id {id} no existe o está inactiva.");

        return filas;
    }

    public async Task<int> ActualizarParcial(
        int id,
        EvaluacionDocenteActualizar peticion)
    {
        var campos = new EvaluacionDocenteCampos(
            peticion.Calificacion,
            peticion.Semestre?.Trim(),
            peticion.Docente);

        if (!campos.HayAlguno)
            throw new ArgumentException(
                "No se envió ningún campo para actualizar.");

        await ObtenerPorId(id);

        if (peticion.Docente.HasValue)
            await ValidarDocente(peticion.Docente.Value);

        var filas = await _repositorio.ActualizarParcial(
            id,
            campos);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                $"La evaluación con id {id} no existe o está inactiva.");

        return filas;
    }

    public async Task<int> Eliminar(int id)
    {
        var filas = await _repositorio.EliminarLogico(id);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                $"La evaluación con id {id} no existe o está inactiva.");

        return filas;
    }

    private async Task ValidarDocente(int cedula)
    {
        var docente = await _docentes.ObtenerPorId(cedula);

        if (docente == null)
            throw new ArgumentException(
                $"El docente con cédula {cedula} no existe o no está activo.");
    }
}
