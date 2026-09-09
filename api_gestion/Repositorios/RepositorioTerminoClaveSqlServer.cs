using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioTerminoClaveSqlServer : IRepositorioTerminoClave
{
    private readonly string _cadenaConexion;

    public RepositorioTerminoClaveSqlServer(IConfiguration configuracion)
    {
        _cadenaConexion =
            configuracion.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'SqlServer'.");
    }

    public async Task<IEnumerable<TerminoClave>> ObtenerTodos(int limite)
    {
        const string sql = """
            SELECT TOP (@Limite)
                termino AS Termino,
                termino_ingles AS TerminoIngles
            FROM termino_clave
            WHERE activo = 1
            ORDER BY termino;
            """;

        await using var conexion = new SqlConnection(_cadenaConexion);

        return await conexion.QueryAsync<TerminoClave>(
            sql,
            new { Limite = limite });
    }

    public async Task<TerminoClave?> ObtenerPorTermino(string termino)
    {
        const string sql = """
            SELECT
                termino AS Termino,
                termino_ingles AS TerminoIngles
            FROM termino_clave
            WHERE termino = @Termino
              AND activo = 1;
            """;

        await using var conexion = new SqlConnection(_cadenaConexion);

        return await conexion.QuerySingleOrDefaultAsync<TerminoClave>(
            sql,
            new { Termino = termino });
    }

    public async Task Crear(TerminoClave terminoClave)
    {
        const string sql = """
            INSERT INTO termino_clave
            (
                termino,
                termino_ingles,
                activo
            )
            VALUES
            (
                @Termino,
                @TerminoIngles,
                1
            );
            """;

        await using var conexion = new SqlConnection(_cadenaConexion);

        await conexion.ExecuteAsync(sql, terminoClave);
    }

    public async Task<int> Reemplazar(TerminoClave terminoClave)
    {
        const string sql = """
            UPDATE termino_clave
            SET termino_ingles = @TerminoIngles
            WHERE termino = @Termino
              AND activo = 1;
            """;

        await using var conexion = new SqlConnection(_cadenaConexion);

        return await conexion.ExecuteAsync(sql, terminoClave);
    }

    public async Task<int> ActualizarParcial(
        string termino,
        TerminoClaveCampos campos)
    {
        const string sql = """
            UPDATE termino_clave
            SET termino_ingles = @TerminoIngles
            WHERE termino = @Termino
              AND activo = 1;
            """;

        await using var conexion = new SqlConnection(_cadenaConexion);

        return await conexion.ExecuteAsync(
            sql,
            new
            {
                Termino = termino,
                campos.TerminoIngles
            });
    }

    public async Task<int> EliminarLogico(string termino)
    {
        const string sql = """
            UPDATE termino_clave
            SET activo = 0
            WHERE termino = @Termino
              AND activo = 1;
            """;

        await using var conexion = new SqlConnection(_cadenaConexion);

        return await conexion.ExecuteAsync(
            sql,
            new { Termino = termino });
    }
}
