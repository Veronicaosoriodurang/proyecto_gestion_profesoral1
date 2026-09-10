using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioApoyoProfesoralSqlServer
    : IRepositorioApoyoProfesoral
{
    private readonly string _cadenaConexion;

    public RepositorioApoyoProfesoralSqlServer(
        IConfiguration configuracion)
    {
        _cadenaConexion =
            configuracion.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'SqlServer'.");
    }

    private IDbConnection CrearConexion() =>
        new SqlConnection(_cadenaConexion);

    public async Task<IEnumerable<ApoyoProfesoral>> ObtenerTodos(
        int limite)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            SELECT TOP (@Limite)
                estudios AS Estudios,
                con_apoyo AS ConApoyo,
                institucion AS Institucion,
                tipo AS Tipo
            FROM apoyo_profesoral
            WHERE activo = 1
            ORDER BY estudios";

        return await conexion.QueryAsync<ApoyoProfesoral>(
            sql,
            new { Limite = limite });
    }

    public async Task<ApoyoProfesoral?> ObtenerPorId(
        int estudios)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            SELECT
                estudios AS Estudios,
                con_apoyo AS ConApoyo,
                institucion AS Institucion,
                tipo AS Tipo
            FROM apoyo_profesoral
            WHERE estudios = @Estudios
              AND activo = 1";

        return await conexion.QueryFirstOrDefaultAsync<ApoyoProfesoral>(
            sql,
            new { Estudios = estudios });
    }

    public async Task<int> Crear(ApoyoProfesoral entidad)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            INSERT INTO apoyo_profesoral
                (
                    estudios,
                    con_apoyo,
                    institucion,
                    tipo,
                    activo
                )
            VALUES
                (
                    @Estudios,
                    @ConApoyo,
                    @Institucion,
                    @Tipo,
                    1
                )";

        return await conexion.ExecuteAsync(sql, entidad);
    }

    public async Task<int> Reemplazar(
        int estudiosActual,
        ApoyoProfesoral entidad)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            UPDATE apoyo_profesoral
            SET estudios = @EstudiosNuevo,
                con_apoyo = @ConApoyo,
                institucion = @Institucion,
                tipo = @Tipo
            WHERE estudios = @EstudiosActual
              AND activo = 1";

        return await conexion.ExecuteAsync(
            sql,
            new
            {
                EstudiosActual = estudiosActual,
                EstudiosNuevo = entidad.Estudios,
                entidad.ConApoyo,
                entidad.Institucion,
                entidad.Tipo
            });
    }

    public async Task<int> ActualizarParcial(
        int estudiosActual,
        ApoyoProfesoralCampos campos)
    {
        using var conexion = CrearConexion();

        var sets = new List<string>();
        var p = new DynamicParameters();

        p.Add("EstudiosActual", estudiosActual);

        if (campos.Estudios.HasValue)
        {
            sets.Add("estudios = @EstudiosNuevo");
            p.Add("EstudiosNuevo", campos.Estudios.Value);
        }

        if (campos.ConApoyo.HasValue)
        {
            sets.Add("con_apoyo = @ConApoyo");
            p.Add("ConApoyo", campos.ConApoyo.Value);
        }

        if (campos.Institucion != null)
        {
            sets.Add("institucion = @Institucion");
            p.Add("Institucion", campos.Institucion);
        }

        if (campos.Tipo != null)
        {
            sets.Add("tipo = @Tipo");
            p.Add("Tipo", campos.Tipo);
        }

        if (sets.Count == 0)
            return 0;

        var sql = $@"
            UPDATE apoyo_profesoral
            SET {string.Join(", ", sets)}
            WHERE estudios = @EstudiosActual
              AND activo = 1";

        return await conexion.ExecuteAsync(sql, p);
    }

    public async Task<int> EliminarLogico(int estudios)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            UPDATE apoyo_profesoral
            SET activo = 0
            WHERE estudios = @Estudios
              AND activo = 1";

        return await conexion.ExecuteAsync(
            sql,
            new { Estudios = estudios });
    }
}
