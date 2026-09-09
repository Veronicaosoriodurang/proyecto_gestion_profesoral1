using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

/// <summary>
/// Capa de negocio para docente_departamento.
/// Valida las relaciones con docente y programa.
/// </summary>
public class ServicioDocenteDepartamento : IServicioDocenteDepartamento
{
    private readonly IRepositorioDocenteDepartamento _repositorio;
    private readonly IRepositorioDocente _docentes;
    private readonly IRepositorioPrograma _programas;

    public ServicioDocenteDepartamento(
        IRepositorioDocenteDepartamento repositorio,
        IRepositorioDocente docentes,
        IRepositorioPrograma programas)
    {
        _repositorio = repositorio;
        _docentes = docentes;
        _programas = programas;
    }

    public async Task<IEnumerable<DocenteDepartamento>> ObtenerTodos(int limite)
    {
        if (limite <= 0)
        {
            throw new ArgumentException(
                "El parámetro limite debe ser un número mayor a 0.");
        }

        return await _repositorio.ObtenerTodos(limite);
    }

    public async Task<DocenteDepartamento> ObtenerPorId(
        int docente,
        int departamento)
    {
        var relacion =
            await _repositorio.ObtenerPorId(docente, departamento);

        if (relacion == null)
        {
            throw new NoEncontradoExcepcion(
                $"No existe la relación entre el docente {docente} " +
                $"y el departamento {departamento}.");
        }

        return relacion;
    }

    public async Task Crear(DocenteDepartamentoCrear peticion)
    {
        var docente = peticion.Docente!.Value;
        var departamento = peticion.Departamento!.Value;

        await ValidarDocente(docente);
        await ValidarPrograma(departamento);

        var relacion = new DocenteDepartamento
        {
            Docente = docente,
            Departamento = departamento,
            Dedicacion = peticion.Dedicacion,
            Modalidad = peticion.Modalidad,
            FechaIngreso = peticion.FechaIngreso!.Value,
            FechaSalida = peticion.FechaSalida
        };

        await _repositorio.Crear(relacion);
    }

    public async Task<int> Reemplazar(
        int docente,
        int departamento,
        DocenteDepartamentoReemplazo peticion)
    {
        await ValidarDocente(docente);
        await ValidarPrograma(departamento);

        var relacion = new DocenteDepartamento
        {
            Docente = docente,
            Departamento = departamento,
            Dedicacion = peticion.Dedicacion,
            Modalidad = peticion.Modalidad,
            FechaIngreso = peticion.FechaIngreso!.Value,
            FechaSalida = peticion.FechaSalida
        };

        var filasAfectadas =
            await _repositorio.Reemplazar(relacion);

        if (filasAfectadas == 0)
        {
            throw new NoEncontradoExcepcion(
                $"No existe la relación entre el docente {docente} " +
                $"y el departamento {departamento}.");
        }

        return filasAfectadas;
    }

    public async Task<int> ActualizarParcial(
        int docente,
        int departamento,
        DocenteDepartamentoActualizar peticion)
    {
        var campos = new DocenteDepartamentoCampos(
            peticion.Dedicacion,
            peticion.Modalidad,
            peticion.FechaIngreso,
            peticion.FechaSalida);

        if (!campos.HayAlguno)
        {
            throw new ArgumentException(
                "No se envió ningún campo para actualizar.");
        }

        await ValidarDocente(docente);
        await ValidarPrograma(departamento);

        var filasAfectadas =
            await _repositorio.ActualizarParcial(
                docente,
                departamento,
                campos);

        if (filasAfectadas == 0)
        {
            throw new NoEncontradoExcepcion(
                $"No existe la relación entre el docente {docente} " +
                $"y el departamento {departamento}.");
        }

        return filasAfectadas;
    }

    public async Task<int> Eliminar(int docente, int departamento)
    {
        var filasAfectadas =
            await _repositorio.EliminarLogico(docente, departamento);

        if (filasAfectadas == 0)
        {
            throw new NoEncontradoExcepcion(
                $"No existe la relación entre el docente {docente} " +
                $"y el departamento {departamento}.");
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

    private async Task ValidarPrograma(int id)
    {
        var programa = await _programas.ObtenerPorId(id);

        if (programa == null)
        {
            throw new ArgumentException(
                $"El departamento/programa con id {id} no existe o no está activo.");
        }
    }
}
