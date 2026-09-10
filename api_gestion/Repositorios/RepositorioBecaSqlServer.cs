using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioBecaSqlServer : IRepositorioBeca
{
    private readonly string _cadenaConexion;

    public RepositorioBecaSqlServer(
        IConfiguration configuracion)
    {
        _cadenaConexion =
            configuracion.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'SqlServer'.");
    }

    private IDbConnection CrearConexion() =>
        new SqlConnection(_cadenaConexion);

    public async Task<IEnumerable<Beca>> ObtenerTodos(
        int limite)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            SELECT TOP (@Limite)
                estudios AS Estudios,
                tipo AS Tipo,
                institucion AS Institucion,
                fecha_inicio AS FechaInicio,
                fecha_fin AS FechaFin
            FROM beca
            WHERE activo = 1
            ORDER BY estudios";

        return await conexion.QueryAsync<Beca>(
            sql,
            new { Limite = limite });
    }

    public async Task<Beca?> ObtenerPorId(
        int estudios)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            SELECT
                estudios AS Estudios,
                tipo AS Tipo,
                institucion AS Institucion,
                fecha_inicio AS FechaInicio,
                fecha_fin AS FechaFin
            FROM beca
            WHERE estudios = @Estudios
              AND activo = 1";

        return await conexion.QueryFirstOrDefaultAsync<Beca>(
            sql,
            new { Estudios = estudios });
    }

    public async Task<int> Crear(Beca entidad)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            INSERT INTO beca
                (
                    estudios,
                    tipo,
                    institucion,
                    fecha_inicio,
                    fecha_fin,
                    activo
                )
            VALUES
                (
                    @Estudios,
                    @Tipo,
                    @Institucion,
                    @FechaInicio,
                    @FechaFin,
                    1
                )";

        return await conexion.ExecuteAsync(
            sql,
            new
            {
                entidad.Estudios,
                entidad.Tipo,
                entidad.Institucion,
                FechaInicio = entidad.FechaInicio.ToDateTime(TimeOnly.MinValue),
                FechaFin = entidad.FechaFin.HasValue
                    ? entidad.FechaFin.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null
            });
    }

    public async Task<int> Reemplazar(
        int estudiosActual,
        Beca entidad)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            UPDATE beca
            SET estudios = @EstudiosNuevo,
                tipo = @Tipo,
                institucion = @Institucion,
                fecha_inicio = @FechaInicio,
                fecha_fin = @FechaFin
            WHERE estudios = @EstudiosActual
              AND activo = 1";

        return await conexion.ExecuteAsync(
            sql,
            new
            {
                EstudiosActual = estudiosActual,
                EstudiosNuevo = entidad.Estudios,
                entidad.Tipo,
                entidad.Institucion,
                FechaInicio = entidad.FechaInicio.ToDateTime(TimeOnly.MinValue),
                FechaFin = entidad.FechaFin.HasValue
                    ? entidad.FechaFin.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null
            });
    }

    public async Task<int> ActualizarParcial(
        int estudiosActual,
        BecaCampos campos)
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

        if (campos.Tipo != null)
        {
            sets.Add("tipo = @Tipo");
            p.Add("Tipo", campos.Tipo);
        }

        if (campos.Institucion != null)
        {
            sets.Add("institucion = @Institucion");
            p.Add("Institucion", campos.Institucion);
        }

        if (campos.FechaInicio.HasValue)
        {
            sets.Add("fecha_inicio = @FechaInicio");
            p.Add(
                "FechaInicio",
                campos.FechaInicio.Value.ToDateTime(TimeOnly.MinValue));
        }

        if (campos.FechaFin.HasValue)
        {
            sets.Add("fecha_fin = @FechaFin");
            p.Add(
                "FechaFin",
                campos.FechaFin.Value.ToDateTime(TimeOnly.MinValue));
        }

        if (sets.Count == 0)
            return 0;

        var sql = $@"
            UPDATE beca
            SET {string.Join(", ", sets)}
            WHERE estudios = @EstudiosActual
              AND activo = 1";

        return await conexion.ExecuteAsync(sql, p);
    }

    public async Task<int> EliminarLogico(
        int estudios)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            UPDATE beca
            SET activo = 0
            WHERE estudios = @Estudios
              AND activo = 1";

        return await conexion.ExecuteAsync(
            sql,
            new { Estudios = estudios });
    }
}
