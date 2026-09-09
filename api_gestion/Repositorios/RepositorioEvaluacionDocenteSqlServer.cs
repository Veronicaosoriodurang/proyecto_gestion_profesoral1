using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioEvaluacionDocenteSqlServer
    : IRepositorioEvaluacionDocente
{
    private readonly string _cadenaConexion;

    public RepositorioEvaluacionDocenteSqlServer(
        IConfiguration configuracion)
    {
        _cadenaConexion =
            configuracion.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'SqlServer'.");
    }

    private IDbConnection CrearConexion() =>
        new SqlConnection(_cadenaConexion);

    public async Task<IEnumerable<EvaluacionDocente>> ObtenerTodos(
        int limite)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            SELECT TOP (@Limite)
                id AS Id,
                calificacion AS Calificacion,
                semestre AS Semestre,
                docente AS Docente
            FROM evaluacion_docente
            WHERE activo = 1
            ORDER BY id";

        return await conexion.QueryAsync<EvaluacionDocente>(
            sql,
            new { Limite = limite });
    }

    public async Task<EvaluacionDocente?> ObtenerPorId(int id)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            SELECT
                id AS Id,
                calificacion AS Calificacion,
                semestre AS Semestre,
                docente AS Docente
            FROM evaluacion_docente
            WHERE id = @Id
              AND activo = 1";

        return await conexion.QueryFirstOrDefaultAsync<EvaluacionDocente>(
            sql,
            new { Id = id });
    }

    public async Task<int> Crear(EvaluacionDocente evaluacion)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            INSERT INTO evaluacion_docente
                (calificacion, semestre, docente, activo)
            OUTPUT INSERTED.id
            VALUES
                (@Calificacion, @Semestre, @Docente, 1)";

        return await conexion.ExecuteScalarAsync<int>(
            sql,
            evaluacion);
    }

    public async Task<int> Reemplazar(
        int id,
        EvaluacionDocente evaluacion)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            UPDATE evaluacion_docente
            SET calificacion = @Calificacion,
                semestre = @Semestre,
                docente = @Docente
            WHERE id = @Id
              AND activo = 1";

        return await conexion.ExecuteAsync(sql, new
        {
            Id = id,
            evaluacion.Calificacion,
            evaluacion.Semestre,
            evaluacion.Docente
        });
    }

    public async Task<int> ActualizarParcial(
        int id,
        EvaluacionDocenteCampos campos)
    {
        using var conexion = CrearConexion();

        var asignaciones = new List<string>();
        var parametros = new DynamicParameters();

        parametros.Add("Id", id);

        if (campos.Calificacion.HasValue)
        {
            asignaciones.Add("calificacion = @Calificacion");
            parametros.Add("Calificacion", campos.Calificacion.Value);
        }

        if (campos.Semestre != null)
        {
            asignaciones.Add("semestre = @Semestre");
            parametros.Add("Semestre", campos.Semestre);
        }

        if (campos.Docente.HasValue)
        {
            asignaciones.Add("docente = @Docente");
            parametros.Add("Docente", campos.Docente.Value);
        }

        if (asignaciones.Count == 0)
            return 0;

        var sql = $@"
            UPDATE evaluacion_docente
            SET {string.Join(", ", asignaciones)}
            WHERE id = @Id
              AND activo = 1";

        return await conexion.ExecuteAsync(sql, parametros);
    }

    public async Task<int> EliminarLogico(int id)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            UPDATE evaluacion_docente
            SET activo = 0
            WHERE id = @Id
              AND activo = 1";

        return await conexion.ExecuteAsync(
            sql,
            new { Id = id });
    }
}
