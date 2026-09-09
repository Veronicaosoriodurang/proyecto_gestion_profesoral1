using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioEstudioAc : IServicioEstudioAc
{
    private readonly IRepositorioEstudioAc _repositorio;
    private readonly IRepositorioEstudioRealizado _estudios;
    private readonly IRepositorioAreaConocimiento _areas;

    public ServicioEstudioAc(
        IRepositorioEstudioAc repositorio,
        IRepositorioEstudioRealizado estudios,
        IRepositorioAreaConocimiento areas)
    {
        _repositorio = repositorio;
        _estudios = estudios;
        _areas = areas;
    }

    public async Task<IEnumerable<EstudioAc>> ObtenerTodos(
        int limite)
    {
        if (limite <= 0)
            throw new ArgumentException(
                "El parámetro limite debe ser mayor a 0.");

        return await _repositorio.ObtenerTodos(limite);
    }

    public async Task<EstudioAc> ObtenerPorId(
        int estudio,
        string areaConocimiento)
    {
        var dato = await _repositorio.ObtenerPorId(
            estudio,
            areaConocimiento);

        if (dato == null)
            throw new NoEncontradoExcepcion(
                "La relación estudio-área no existe o está inactiva.");

        return dato;
    }

    public async Task<int> Crear(
        EstudioAcCrear peticion)
    {
        var estudio = peticion.Estudio!.Value;
        var area = peticion.AreaConocimiento.Trim();

        await ValidarEstudio(estudio);
        await ValidarArea(area);

        return await _repositorio.Crear(
            new EstudioAc
            {
                Estudio = estudio,
                AreaConocimiento = area
            });
    }

    public async Task<int> Reemplazar(
        int estudio,
        string areaConocimiento,
        EstudioAcReemplazo peticion)
    {
        await ObtenerPorId(estudio, areaConocimiento);

        var estudioNuevo = peticion.Estudio!.Value;
        var areaNueva = peticion.AreaConocimiento.Trim();

        await ValidarEstudio(estudioNuevo);
        await ValidarArea(areaNueva);

        var filas = await _repositorio.Reemplazar(
            estudio,
            areaConocimiento,
            new EstudioAc
            {
                Estudio = estudioNuevo,
                AreaConocimiento = areaNueva
            });

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                "La relación estudio-área no existe o está inactiva.");

        return filas;
    }

    public async Task<int> ActualizarParcial(
        int estudio,
        string areaConocimiento,
        EstudioAcActualizar peticion)
    {
        var campos = new EstudioAcCampos(
            peticion.Estudio,
            peticion.AreaConocimiento?.Trim());

        if (!campos.HayAlguno)
            throw new ArgumentException(
                "No se envió ningún campo para actualizar.");

        await ObtenerPorId(estudio, areaConocimiento);

        if (peticion.Estudio.HasValue)
            await ValidarEstudio(peticion.Estudio.Value);

        if (peticion.AreaConocimiento != null)
            await ValidarArea(
                peticion.AreaConocimiento.Trim());

        var filas =
            await _repositorio.ActualizarParcial(
                estudio,
                areaConocimiento,
                campos);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                "La relación estudio-área no existe o está inactiva.");

        return filas;
    }

    public async Task<int> Eliminar(
        int estudio,
        string areaConocimiento)
    {
        var filas =
            await _repositorio.EliminarLogico(
                estudio,
                areaConocimiento);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                "La relación estudio-área no existe o está inactiva.");

        return filas;
    }

    private async Task ValidarEstudio(int id)
    {
        if (await _estudios.ObtenerPorId(id) == null)
            throw new ArgumentException(
                $"El estudio con id {id} no existe o no está activo.");
    }

    private async Task ValidarArea(string id)
    {
        if (await _areas.ObtenerPorId(id) == null)
            throw new ArgumentException(
                $"El área de conocimiento '{id}' no existe o no está activa.");
    }
}
