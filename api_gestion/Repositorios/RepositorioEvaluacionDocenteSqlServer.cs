using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

// SQL vive en db/version_2.sql. Cada método llama un procedimiento explícito.
public class RepositorioEvaluacionDocenteSqlServer : IRepositorioEvaluacionDocente
{
    static RepositorioEvaluacionDocenteSqlServer() => FechasSql.Configurar();
    private readonly string cadena;
    public RepositorioEvaluacionDocenteSqlServer(IConfiguration configuracion) => cadena = configuracion.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("Falta la conexión SqlServer.");

    internal static DynamicParameters Parametros(EvaluacionDocente entidad)
    {
        var p = new DynamicParameters();
        p.Add("Calificacion", entidad.Calificacion);
        p.Add("Semestre", entidad.Semestre);
        p.Add("Docente", entidad.Docente);
        return p;
    }

    public async Task<IEnumerable<EvaluacionDocente>> ObtenerTodos(int limite)
    {
        await using var conexion = new SqlConnection(cadena);
        return await conexion.QueryAsync<EvaluacionDocente>("dbo.sp_listar_evaluacion_docente", new { Limite = limite }, commandType: CommandType.StoredProcedure);
    }

    public async Task<EvaluacionDocente?> ObtenerPorId(int id)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Id", id);
        return await conexion.QuerySingleOrDefaultAsync<EvaluacionDocente>("dbo.sp_consultar_evaluacion_docente", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> Crear(EvaluacionDocente evaluacion)
    {
        await using var conexion = new SqlConnection(cadena);
        var guardada = await conexion.QuerySingleAsync<EvaluacionDocente>("dbo.sp_crear_evaluacion_docente", Parametros(evaluacion), commandType: CommandType.StoredProcedure);
        evaluacion.Id = guardada.Id;
        evaluacion.Calificacion = guardada.Calificacion;
        evaluacion.Semestre = guardada.Semestre;
        evaluacion.Docente = guardada.Docente;
        return 1;
    }

    public async Task<int> Reemplazar(int id, EvaluacionDocente evaluacion)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = Parametros(evaluacion);
        p.Add("OriginalId", id);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_evaluacion_docente", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> ActualizarParcial(int id, EvaluacionDocenteCampos campos)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Calificacion", campos.Calificacion);
        p.Add("Semestre", campos.Semestre);
        p.Add("Docente", campos.Docente);
        p.Add("Parcial", true);
        p.Add("OriginalId", id);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_evaluacion_docente", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> EliminarLogico(int id)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Id", id);
        return await conexion.QuerySingleAsync<int>("dbo.sp_eliminar_evaluacion_docente", p, commandType: CommandType.StoredProcedure);
    }
}
