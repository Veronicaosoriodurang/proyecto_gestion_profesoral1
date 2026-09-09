using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioEstudioAcSqlServer
    : IRepositorioEstudioAc
{
    private readonly string _cadenaConexion;

    public RepositorioEstudioAcSqlServer(
        IConfiguration configuracion)
    {
        _cadenaConexion =
            configuracion.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'SqlServer'.");
    }

    private IDbConnection CrearConexion() =>
        new SqlConnection(_cadenaConexion);

    public async Task<IEnumerable<EstudioAc>> ObtenerTodos(
        int limite)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            SELECT TOP (@Limite)
                estudio AS Estudio,
                area_conocimiento AS AreaConocimiento
            FROM estudio_ac
            WHERE activo = 1
            ORDER BY estudio, area_conocimiento";

        return await conexion.QueryAsync<EstudioAc>(
            sql,
            new { Limite = limite });
    }

    public async Task<EstudioAc?> ObtenerPorId(
        int estudio,
        string areaConocimiento)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            SELECT
                estudio AS Estudio,
                area_conocimiento AS AreaConocimiento
            FROM estudio_ac
            WHERE estudio = @Estudio
              AND area_conocimiento = @AreaConocimiento
              AND activo = 1";

        return await conexion.QueryFirstOrDefaultAsync<EstudioAc>(
            sql,
            new
            {
                Estudio = estudio,
                AreaConocimiento = areaConocimiento
            });
    }

    public async Task<int> Crear(EstudioAc entidad)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            INSERT INTO estudio_ac
                (
                    estudio,
                    area_conocimiento,
                    activo
                )
            VALUES
                (
                    @Estudio,
                    @AreaConocimiento,
                    1
                )";

        return await conexion.ExecuteAsync(
            sql,
            new
            {
                entidad.Estudio,
                entidad.AreaConocimiento
            });
    }

    public async Task<int> Reemplazar(
        int estudioActual,
        string areaActual,
        EstudioAc entidad)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            UPDATE estudio_ac
            SET estudio = @EstudioNuevo,
                area_conocimiento = @AreaNueva
            WHERE estudio = @EstudioActual
              AND area_conocimiento = @AreaActual
              AND activo = 1";

        return await conexion.ExecuteAsync(
            sql,
            new
            {
                EstudioActual = estudioActual,
                AreaActual = areaActual,
                EstudioNuevo = entidad.Estudio,
                AreaNueva = entidad.AreaConocimiento
            });
    }

    public async Task<int> ActualizarParcial(
        int estudioActual,
        string areaActual,
        EstudioAcCampos campos)
    {
        using var conexion = CrearConexion();

        var sets = new List<string>();
        var p = new DynamicParameters();

        p.Add("EstudioActual", estudioActual);
        p.Add("AreaActual", areaActual);

        if (campos.Estudio.HasValue)
        {
            sets.Add("estudio = @EstudioNuevo");
            p.Add("EstudioNuevo", campos.Estudio.Value);
        }

        if (campos.AreaConocimiento != null)
        {
            sets.Add("area_conocimiento = @AreaNueva");
            p.Add("AreaNueva", campos.AreaConocimiento);
        }

        if (sets.Count == 0)
            return 0;

        var sql = $@"
            UPDATE estudio_ac
            SET {string.Join(", ", sets)}
            WHERE estudio = @EstudioActual
              AND area_conocimiento = @AreaActual
              AND activo = 1";

        return await conexion.ExecuteAsync(sql, p);
    }

    public async Task<int> EliminarLogico(
        int estudio,
        string areaConocimiento)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            UPDATE estudio_ac
            SET activo = 0
            WHERE estudio = @Estudio
              AND area_conocimiento = @AreaConocimiento
              AND activo = 1";

        return await conexion.ExecuteAsync(
            sql,
            new
            {
                Estudio = estudio,
                AreaConocimiento = areaConocimiento
            });
    }
}
