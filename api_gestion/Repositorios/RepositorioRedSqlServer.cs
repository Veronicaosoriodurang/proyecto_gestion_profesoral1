using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioRedSqlServer : IRepositorioRed
{
    private readonly string _cadenaConexion;

    public RepositorioRedSqlServer(IConfiguration configuracion)
    {
        _cadenaConexion =
            configuracion.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("No se encontrÃ³ la cadena de conexiÃ³n.");
    }

    private IDbConnection CrearConexion()
    {
        return new SqlConnection(_cadenaConexion);
    }

    public async Task<IEnumerable<Red>> ObtenerTodos(int limite)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            SELECT TOP (@Limite)
                idr AS Idr,
                nombre AS Nombre,
                url AS Url,
                pais AS Pais
            FROM red
            WHERE activo = 1
            ORDER BY idr";

        return await conexion.QueryAsync<Red>(sql, new { Limite = limite });
    }

    public async Task<Red?> ObtenerPorId(int idr)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            SELECT
                idr AS Idr,
                nombre AS Nombre,
                url AS Url,
                pais AS Pais
            FROM red
            WHERE idr = @Idr
              AND activo = 1";

        return await conexion.QueryFirstOrDefaultAsync<Red>(sql, new { Idr = idr });
    }

    public async Task Crear(Red red)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            INSERT INTO red (idr, nombre, url, pais, activo)
            VALUES (@Idr, @Nombre, @Url, @Pais, 1)";

        await conexion.ExecuteAsync(sql, red);
    }

    public async Task<int> Reemplazar(Red red)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            UPDATE red
            SET nombre = @Nombre,
                url = @Url,
                pais = @Pais
            WHERE idr = @Idr
              AND activo = 1";

        return await conexion.ExecuteAsync(sql, red);
    }

    public async Task<int> ActualizarParcial(int idr, RedCampos campos)
    {
        using var conexion = CrearConexion();

        var asignaciones = new List<string>();
        var parametros = new DynamicParameters();
        parametros.Add("Idr", idr);

        void Agregar(string columna, string parametro, object? valor)
        {
            if (valor == null) return;

            asignaciones.Add($"{columna} = @{parametro}");
            parametros.Add(parametro, valor);
        }

        Agregar("nombre", "Nombre", campos.Nombre);
        Agregar("url", "Url", campos.Url);
        Agregar("pais", "Pais", campos.Pais);

        if (asignaciones.Count == 0)
        {
            return 0;
        }

        var sql =
            $"UPDATE red SET {string.Join(", ", asignaciones)} " +
            "WHERE idr = @Idr AND activo = 1";

        return await conexion.ExecuteAsync(sql, parametros);
    }

    public async Task<int> EliminarLogico(int idr)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            UPDATE red
            SET activo = 0
            WHERE idr = @Idr
              AND activo = 1";

        return await conexion.ExecuteAsync(sql, new { Idr = idr });
    }
}
