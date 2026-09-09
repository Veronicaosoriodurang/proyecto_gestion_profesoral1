using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioAreaConocimientoSqlServer : IRepositorioAreaConocimiento
{
    private readonly string _cadenaConexion;

    public RepositorioAreaConocimientoSqlServer(IConfiguration configuracion)
    {
        _cadenaConexion =
            configuracion.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'SqlServer'.");
    }

    public async Task<IEnumerable<AreaConocimiento>> ObtenerTodos(int limite)
    {
        const string sql = """
            SELECT TOP (@Limite)
                id AS Id,
                gran_area AS GranArea,
                area AS Area,
                disciplina AS Disciplina
            FROM area_conocimiento
            WHERE activo = 1
            ORDER BY id;
            """;

        await using var conexion = new SqlConnection(_cadenaConexion);

        return await conexion.QueryAsync<AreaConocimiento>(
            sql,
            new { Limite = limite });
    }

    public async Task<AreaConocimiento?> ObtenerPorId(string id)
    {
        const string sql = """
            SELECT
                id AS Id,
                gran_area AS GranArea,
                area AS Area,
                disciplina AS Disciplina
            FROM area_conocimiento
            WHERE id = @Id
              AND activo = 1;
            """;

        await using var conexion = new SqlConnection(_cadenaConexion);

        return await conexion.QuerySingleOrDefaultAsync<AreaConocimiento>(
            sql,
            new { Id = id });
    }

    public async Task Crear(AreaConocimiento areaConocimiento)
    {
        const string sql = """
            INSERT INTO area_conocimiento
            (
                id,
                gran_area,
                area,
                disciplina,
                activo
            )
            VALUES
            (
                @Id,
                @GranArea,
                @Area,
                @Disciplina,
                1
            );
            """;

        await using var conexion = new SqlConnection(_cadenaConexion);

        await conexion.ExecuteAsync(sql, areaConocimiento);
    }

    public async Task<int> Reemplazar(AreaConocimiento areaConocimiento)
    {
        const string sql = """
            UPDATE area_conocimiento
            SET
                gran_area = @GranArea,
                area = @Area,
                disciplina = @Disciplina
            WHERE id = @Id
              AND activo = 1;
            """;

        await using var conexion = new SqlConnection(_cadenaConexion);

        return await conexion.ExecuteAsync(sql, areaConocimiento);
    }

    public async Task<int> ActualizarParcial(
        string id,
        AreaConocimientoCampos campos)
    {
        var cambios = new List<string>();

        var parametros = new DynamicParameters();
        parametros.Add("Id", id);

        if (campos.GranArea != null)
        {
            cambios.Add("gran_area = @GranArea");
            parametros.Add("GranArea", campos.GranArea);
        }

        if (campos.Area != null)
        {
            cambios.Add("area = @Area");
            parametros.Add("Area", campos.Area);
        }

        if (campos.Disciplina != null)
        {
            cambios.Add("disciplina = @Disciplina");
            parametros.Add("Disciplina", campos.Disciplina);
        }

        if (cambios.Count == 0)
            return 0;

        var sql = $"""
            UPDATE area_conocimiento
            SET {string.Join(", ", cambios)}
            WHERE id = @Id
              AND activo = 1;
            """;

        await using var conexion = new SqlConnection(_cadenaConexion);

        return await conexion.ExecuteAsync(sql, parametros);
    }

    public async Task<int> EliminarLogico(string id)
    {
        const string sql = """
            UPDATE area_conocimiento
            SET activo = 0
            WHERE id = @Id
              AND activo = 1;
            """;

        await using var conexion = new SqlConnection(_cadenaConexion);

        return await conexion.ExecuteAsync(sql, new { Id = id });
    }
}
