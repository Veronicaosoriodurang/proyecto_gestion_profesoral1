using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioApoyoProfesoral : IServicioApoyoProfesoral
{
    private readonly IRepositorioApoyoProfesoral _repositorio;
    private readonly IRepositorioEstudioRealizado _estudios;

    public ServicioApoyoProfesoral(
        IRepositorioApoyoProfesoral repositorio,
        IRepositorioEstudioRealizado estudios)
    {
        _repositorio = repositorio;
        _estudios = estudios;
    }

    public async Task<IEnumerable<ApoyoProfesoral>> ObtenerTodos(
        int limite)
    {
        if (limite <= 0)
            throw new ArgumentException(
                "El parámetro limite debe ser mayor a 0.");

        return await _repositorio.ObtenerTodos(limite);
    }

    public async Task<ApoyoProfesoral> ObtenerPorId(
        int estudios)
    {
        var dato = await _repositorio.ObtenerPorId(estudios);

        if (dato == null)
            throw new NoEncontradoExcepcion(
                "El apoyo profesoral no existe o está inactivo.");

        return dato;
    }

    public async Task<int> Crear(
        ApoyoProfesoralCrear peticion)
    {
        var estudio = peticion.Estudios!.Value;

        await ValidarEstudio(estudio);
        ValidarConApoyo(peticion.ConApoyo!.Value);

        return await _repositorio.Crear(
            new ApoyoProfesoral
            {
                Estudios = estudio,
                ConApoyo = peticion.ConApoyo.Value,
                Institucion = peticion.Institucion.Trim(),
                Tipo = peticion.Tipo.Trim()
            });
    }

    public async Task<int> Reemplazar(
        int estudios,
        ApoyoProfesoralReemplazo peticion)
    {
        await ObtenerPorId(estudios);

        var estudioNuevo = peticion.Estudios!.Value;

        await ValidarEstudio(estudioNuevo);
        ValidarConApoyo(peticion.ConApoyo!.Value);

        var filas = await _repositorio.Reemplazar(
            estudios,
            new ApoyoProfesoral
            {
                Estudios = estudioNuevo,
                ConApoyo = peticion.ConApoyo.Value,
                Institucion = peticion.Institucion.Trim(),
                Tipo = peticion.Tipo.Trim()
            });

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                "El apoyo profesoral no existe o está inactivo.");

        return filas;
    }

    public async Task<int> ActualizarParcial(
        int estudios,
        ApoyoProfesoralActualizar peticion)
    {
        var campos = new ApoyoProfesoralCampos(
            peticion.Estudios,
            peticion.ConApoyo,
            peticion.Institucion?.Trim(),
            peticion.Tipo?.Trim());

        if (!campos.HayAlguno)
            throw new ArgumentException(
                "No se envió ningún campo para actualizar.");

        await ObtenerPorId(estudios);

        if (peticion.Estudios.HasValue)
            await ValidarEstudio(peticion.Estudios.Value);

        if (peticion.ConApoyo.HasValue)
            ValidarConApoyo(peticion.ConApoyo.Value);

        var filas = await _repositorio.ActualizarParcial(
            estudios,
            campos);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                "El apoyo profesoral no existe o está inactivo.");

        return filas;
    }

    public async Task<int> Eliminar(int estudios)
    {
        var filas = await _repositorio.EliminarLogico(estudios);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                "El apoyo profesoral no existe o está inactivo.");

        return filas;
    }

    private async Task ValidarEstudio(int id)
    {
        if (await _estudios.ObtenerPorId(id) == null)
            throw new ArgumentException(
                $"El estudio con id {id} no existe o no está activo.");
    }

    private static void ValidarConApoyo(byte valor)
    {
        if (valor != 0 && valor != 1)
            throw new ArgumentException(
                "El campo conApoyo solo puede ser 0 o 1.");
    }
}
