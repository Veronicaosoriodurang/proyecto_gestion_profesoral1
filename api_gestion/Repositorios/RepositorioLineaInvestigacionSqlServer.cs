using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioLineaInvestigacionSqlServer : IRepositorioLineaInvestigacion
{
    private readonly string _cadenaConexion;

    public RepositorioLineaInvestigacionSqlServer(IConfiguration configuracion)
    {
        _cadenaConexion =
            configuracion.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'SqlServer'.");
    }

    public async Task<IEnumerable<LineaInvestigacion>> ObtenerTodos(int limite)
    {
        const string sql = """
            SELECT TOP (@Limite)
                id AS Id,
                nombre AS Nombre,
                descripcion AS Descripcion
            FROM linea_investigacion
            WHERE activo = 1
            ORDER BY id;
            """;

        await using var conexion = new SqlConnection(_cadenaConexion);

        return await conexion.QueryAsync<LineaInvestigacion>(
            sql,
            new { Limite = limite });
    }

    public async Task<LineaInvestigacion?> ObtenerPorId(int id)
    {
        const string sql = """
            SELECT
                id AS Id,
                nombre AS Nombre,
                descripcion AS Descripcion
            FROM linea_investigacion
            WHERE id = @Id
              AND activo = 1;
            """;

        await using var conexion = new SqlConnection(_cadenaConexion);

        return await conexion.QuerySingleOrDefaultAsync<LineaInvestigacion>(
            sql,
            new { Id = id });
    }

    public async Task Crear(LineaInvestigacion linea)
    {
        const string sql = """
            INSERT INTO linea_investigacion
            (
                nombre,
                descripcion,
                activo
            )
            VALUES
            (
                @Nombre,
                @Descripcion,
                1
            );
            """;

        await using var conexion = new SqlConnection(_cadenaConexion);

        await conexion.ExecuteAsync(sql, linea);
    }

    public async Task<int> Reemplazar(LineaInvestigacion linea)
    {
        const string sql = """
            UPDATE linea_investigacion
            SET
                nombre = @Nombre,
                descripcion = @Descripcion
            WHERE id = @Id
              AND activo = 1;
            """;

        await using var conexion = new SqlConnection(_cadenaConexion);

        return await conexion.ExecuteAsync(sql, linea);
    }

    public async Task<int> ActualizarParcial(
        int id,
        LineaInvestigacionCampos campos)
    {
        var cambios = new List<string>();

        var parametros = new DynamicParameters();
        parametros.Add("Id", id);

        if (campos.Nombre != null)
        {
            cambios.Add("nombre = @Nombre");
            parametros.Add("Nombre", campos.Nombre);
        }

        if (campos.Descripcion != null)
        {
            cambios.Add("descripcion = @Descripcion");
            parametros.Add("Descripcion", campos.Descripcion);
        }

        if (cambios.Count == 0)
            return 0;

        var sql = $"""
            UPDATE linea_investigacion
            SET {string.Join(", ", cambios)}
            WHERE id = @Id
              AND activo = 1;
            """;

        await using var conexion = new SqlConnection(_cadenaConexion);

        return await conexion.ExecuteAsync(sql, parametros);
    }

    public async Task<int> EliminarLogico(int id)
    {
        const string sql = """
            UPDATE linea_investigacion
            SET activo = 0
            WHERE id = @Id
              AND activo = 1;
            """;

        await using var conexion = new SqlConnection(_cadenaConexion);

        return await conexion.ExecuteAsync(
            sql,
            new { Id = id });
    }
}

