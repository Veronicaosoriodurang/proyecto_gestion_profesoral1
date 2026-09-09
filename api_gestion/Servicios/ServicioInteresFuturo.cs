using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioInteresFuturo : IServicioInteresFuturo
{
    private readonly IRepositorioInteresFuturo _repositorio;
    private readonly IRepositorioDocente _docentes;
    private readonly IRepositorioTerminoClave _terminos;

    public ServicioInteresFuturo(
        IRepositorioInteresFuturo repositorio,
        IRepositorioDocente docentes,
        IRepositorioTerminoClave terminos)
    {
        _repositorio = repositorio;
        _docentes = docentes;
        _terminos = terminos;
    }

    public async Task<IEnumerable<InteresFuturo>> ObtenerTodos(int limite)
    {
        if (limite <= 0)
            throw new ArgumentException(
                "El parámetro limite debe ser un número mayor a 0.");

        return await _repositorio.ObtenerTodos(limite);
    }

    public async Task<InteresFuturo> ObtenerPorId(
        int docente,
        string terminoClave)
    {
        var interes =
            await _repositorio.ObtenerPorId(docente, terminoClave);

        if (interes == null)
            throw new NoEncontradoExcepcion(
                $"No existe el interés futuro del docente {docente} " +
                $"con el término '{terminoClave}'.");

        return interes;
    }

    public async Task Crear(InteresFuturoCrear peticion)
    {
        var docente = peticion.Docente!.Value;
        var termino = peticion.TerminoClave.Trim();

        await ValidarDocente(docente);
        await ValidarTermino(termino);

        await _repositorio.Crear(new InteresFuturo
        {
            Docente = docente,
            TerminoClave = termino
        });
    }

    public async Task<int> Reemplazar(
        int docenteActual,
        string terminoActual,
        InteresFuturoReemplazo peticion)
    {
        await ObtenerPorId(docenteActual, terminoActual);

        var nuevoDocente = peticion.Docente!.Value;
        var nuevoTermino = peticion.TerminoClave.Trim();

        await ValidarDocente(nuevoDocente);
        await ValidarTermino(nuevoTermino);

        var filas = await _repositorio.Reemplazar(
            docenteActual,
            terminoActual,
            new InteresFuturo
            {
                Docente = nuevoDocente,
                TerminoClave = nuevoTermino
            });

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                "El interés futuro no existe o está inactivo.");

        return filas;
    }

    public async Task<int> ActualizarParcial(
        int docenteActual,
        string terminoActual,
        InteresFuturoActualizar peticion)
    {
        if (!peticion.Docente.HasValue
            && peticion.TerminoClave == null)
        {
            throw new ArgumentException(
                "No se envió ningún campo para actualizar.");
        }

        await ObtenerPorId(docenteActual, terminoActual);

        if (peticion.Docente.HasValue)
            await ValidarDocente(peticion.Docente.Value);

        string? termino = null;

        if (peticion.TerminoClave != null)
        {
            termino = peticion.TerminoClave.Trim();
            await ValidarTermino(termino);
        }

        var campos = new InteresFuturoCampos(
            peticion.Docente,
            termino);

        var filas =
            await _repositorio.ActualizarParcial(
                docenteActual,
                terminoActual,
                campos);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                "El interés futuro no existe o está inactivo.");

        return filas;
    }

    public async Task<int> Eliminar(
        int docente,
        string terminoClave)
    {
        var filas =
            await _repositorio.EliminarLogico(
                docente,
                terminoClave);

        if (filas == 0)
            throw new NoEncontradoExcepcion(
                $"No existe el interés futuro del docente {docente} " +
                $"con el término '{terminoClave}'.");

        return filas;
    }

    private async Task ValidarDocente(int cedula)
    {
        if (await _docentes.ObtenerPorId(cedula) == null)
            throw new ArgumentException(
                $"El docente con cédula {cedula} no existe o no está activo.");
    }

    private async Task ValidarTermino(string termino)
    {
        if (await _terminos.ObtenerPorTermino(termino) == null)
            throw new ArgumentException(
                $"El término clave '{termino}' no existe o no está activo.");
    }
}
