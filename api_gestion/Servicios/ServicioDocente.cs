using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

/// <summary>
/// Capa 2 de docente. Valida la FK a linea_investigacion (existente y activa)
/// antes de persistir.
/// </summary>
public class ServicioDocente : IServicioDocente
{
    private readonly IRepositorioDocente _repositorio;
    private readonly IRepositorioLineaInvestigacion _lineas;

    public ServicioDocente(
        IRepositorioDocente repositorio,
        IRepositorioLineaInvestigacion lineas)
    {
        _repositorio = repositorio;
        _lineas = lineas;
    }

    public async Task<IEnumerable<Docente>> ObtenerTodos(int limite)
    {
        if (limite <= 0)
        {
            throw new ArgumentException("El parámetro limite debe ser un número mayor a 0.");
        }

        return await _repositorio.ObtenerTodos(limite);
    }

    public async Task<Docente> ObtenerPorId(int cedula)
    {
        var docente = await _repositorio.ObtenerPorId(cedula);
        if (docente == null)
        {
            throw new NoEncontradoExcepcion($"No existe un docente con la cédula {cedula}.");
        }

        return docente;
    }

    public async Task Crear(Docente docente)
    {
        await ValidarLineaSiAplica(docente.LineaInvestigacionPrincipal);
        await _repositorio.Crear(docente);
    }

    public async Task<int> Reemplazar(int cedula, Docente docente)
    {
        docente.Cedula = cedula;
        await ValidarLineaSiAplica(docente.LineaInvestigacionPrincipal);

        var filasAfectadas = await _repositorio.Reemplazar(docente);
        if (filasAfectadas == 0)
        {
            throw new NoEncontradoExcepcion($"No existe un docente con la cédula {cedula}.");
        }

        return filasAfectadas;
    }

    public async Task<int> ActualizarParcial(int cedula, DocenteCampos campos)
    {
        if (!campos.HayAlguno)
        {
            throw new ArgumentException("No se envió ningún campo para actualizar.");
        }

        await ValidarLineaSiAplica(campos.LineaInvestigacionPrincipal);

        var filasAfectadas = await _repositorio.ActualizarParcial(cedula, campos);
        if (filasAfectadas == 0)
        {
            throw new NoEncontradoExcepcion($"No existe un docente con la cédula {cedula}.");
        }

        return filasAfectadas;
    }

    public async Task<int> Eliminar(int cedula)
    {
        var filasAfectadas = await _repositorio.EliminarLogico(cedula);
        if (filasAfectadas == 0)
        {
            throw new NoEncontradoExcepcion($"No existe un docente con la cédula {cedula}.");
        }

        return filasAfectadas;
    }

    /// <summary>
    /// ObtenerPorId de la línea ya filtra activo=1: null implica inexistente o inactiva.
    /// </summary>
    private async Task ValidarLineaSiAplica(int? lineaId)
    {
        if (!lineaId.HasValue) return;

        var linea = await _lineas.ObtenerPorId(lineaId.Value);
        if (linea == null)
        {
            throw new ArgumentException(
                $"La línea de investigación {lineaId.Value} no existe o no está activa.");
        }
    }
}
