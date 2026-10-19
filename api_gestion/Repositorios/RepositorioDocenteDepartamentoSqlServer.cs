using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

// SQL vive en db/version_2.sql. Cada método llama un procedimiento explícito.
public class RepositorioDocenteDepartamentoSqlServer : IRepositorioDocenteDepartamento
{
    static RepositorioDocenteDepartamentoSqlServer() => FechasSql.Configurar();
    private readonly string cadena;
    public RepositorioDocenteDepartamentoSqlServer(IConfiguration configuracion) => cadena = configuracion.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("Falta la conexión SqlServer.");

    internal static DynamicParameters Parametros(DocenteDepartamento entidad)
    {
        var p = new DynamicParameters();
        p.Add("Docente", entidad.Docente);
        p.Add("Departamento", entidad.Departamento);
        p.Add("Dedicacion", entidad.Dedicacion);
        p.Add("Modalidad", entidad.Modalidad);
        p.Add("FechaIngreso", entidad.FechaIngreso.ToDateTime(TimeOnly.MinValue));
        p.Add("FechaSalida", entidad.FechaSalida?.ToDateTime(TimeOnly.MinValue));
        return p;
    }

    public async Task<IEnumerable<DocenteDepartamento>> ObtenerTodos(int limite)
    {
        await using var conexion = new SqlConnection(cadena);
        return await conexion.QueryAsync<DocenteDepartamento>("dbo.sp_listar_docente_departamento", new { Limite = limite }, commandType: CommandType.StoredProcedure);
    }

    public async Task<DocenteDepartamento?> ObtenerPorId(int docente, int departamento)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Docente", docente);
        p.Add("Departamento", departamento);
        return await conexion.QuerySingleOrDefaultAsync<DocenteDepartamento>("dbo.sp_consultar_docente_departamento", p, commandType: CommandType.StoredProcedure);
    }

    public async Task Crear(DocenteDepartamento relacion)
    {
        await using var conexion = new SqlConnection(cadena);
        var guardada = await conexion.QuerySingleAsync<DocenteDepartamento>("dbo.sp_crear_docente_departamento", Parametros(relacion), commandType: CommandType.StoredProcedure);
        relacion.Docente = guardada.Docente;
        relacion.Departamento = guardada.Departamento;
        relacion.Dedicacion = guardada.Dedicacion;
        relacion.Modalidad = guardada.Modalidad;
        relacion.FechaIngreso = guardada.FechaIngreso;
        relacion.FechaSalida = guardada.FechaSalida;
    }

    public async Task<int> Reemplazar(DocenteDepartamento relacion)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = Parametros(relacion);
        p.Add("OriginalDocente", relacion.Docente);
        p.Add("OriginalDepartamento", relacion.Departamento);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_docente_departamento", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> ActualizarParcial(int docente, int departamento, DocenteDepartamentoCampos campos)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Dedicacion", campos.Dedicacion);
        p.Add("Modalidad", campos.Modalidad);
        p.Add("FechaIngreso", campos.FechaIngreso?.ToDateTime(TimeOnly.MinValue));
        p.Add("FechaSalida", campos.FechaSalida?.ToDateTime(TimeOnly.MinValue));
        p.Add("Docente", docente);
        p.Add("Departamento", departamento);
        p.Add("Parcial", true);
        p.Add("OriginalDocente", docente);
        p.Add("OriginalDepartamento", departamento);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_docente_departamento", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> EliminarLogico(int docente, int departamento)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Docente", docente);
        p.Add("Departamento", departamento);
        return await conexion.QuerySingleAsync<int>("dbo.sp_eliminar_docente_departamento", p, commandType: CommandType.StoredProcedure);
    }
}
