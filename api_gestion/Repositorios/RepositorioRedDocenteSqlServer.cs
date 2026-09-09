using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioRedDocenteSqlServer
    : IRepositorioRedDocente
{
    private readonly string _cadenaConexion;

    public RepositorioRedDocenteSqlServer(
        IConfiguration configuracion)
    {
        _cadenaConexion =
            configuracion.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'SqlServer'.");
    }

    private IDbConnection CrearConexion() =>
        new SqlConnection(_cadenaConexion);

    public async Task<IEnumerable<RedDocente>> ObtenerTodos(
        int limite)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            SELECT TOP (@Limite)
                red AS Red,
                docente AS Docente,
                fecha_inicio AS FechaInicio,
                fecha_fin AS FechaFin,
                act_destacadas AS ActDestacadas
            FROM red_docente
            WHERE activo = 1
            ORDER BY red, docente";

        return await conexion.QueryAsync<RedDocente>(
            sql,
            new { Limite = limite });
    }

    public async Task<RedDocente?> ObtenerPorId(
        int red,
        int docente)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            SELECT
                red AS Red,
                docente AS Docente,
                fecha_inicio AS FechaInicio,
                fecha_fin AS FechaFin,
                act_destacadas AS ActDestacadas
            FROM red_docente
            WHERE red = @Red
              AND docente = @Docente
              AND activo = 1";

        return await conexion.QueryFirstOrDefaultAsync<RedDocente>(
            sql,
            new
            {
                Red = red,
                Docente = docente
            });
    }

    public async Task<int> Crear(RedDocente entidad)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            INSERT INTO red_docente
                (
                    red,
                    docente,
                    fecha_inicio,
                    fecha_fin,
                    act_destacadas,
                    activo
                )
            VALUES
                (
                    @Red,
                    @Docente,
                    @FechaInicio,
                    @FechaFin,
                    @ActDestacadas,
                    1
                )";

        return await conexion.ExecuteAsync(
            sql,
            new
            {
                entidad.Red,
                entidad.Docente,
                FechaInicio =
                    entidad.FechaInicio.ToDateTime(
                        TimeOnly.MinValue),
                entidad.FechaFin,
                entidad.ActDestacadas
            });
    }

    public async Task<int> Reemplazar(
        int redActual,
        int docenteActual,
        RedDocente entidad)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            UPDATE red_docente
            SET red = @RedNueva,
                docente = @DocenteNuevo,
                fecha_inicio = @FechaInicio,
                fecha_fin = @FechaFin,
                act_destacadas = @ActDestacadas
            WHERE red = @RedActual
              AND docente = @DocenteActual
              AND activo = 1";

        return await conexion.ExecuteAsync(
            sql,
            new
            {
                RedActual = redActual,
                DocenteActual = docenteActual,
                RedNueva = entidad.Red,
                DocenteNuevo = entidad.Docente,
                FechaInicio =
                    entidad.FechaInicio.ToDateTime(
                        TimeOnly.MinValue),
                entidad.FechaFin,
                entidad.ActDestacadas
            });
    }

    public async Task<int> ActualizarParcial(
        int redActual,
        int docenteActual,
        RedDocenteCampos campos)
    {
        using var conexion = CrearConexion();

        var sets = new List<string>();
        var p = new DynamicParameters();

        p.Add("RedActual", redActual);
        p.Add("DocenteActual", docenteActual);

        if (campos.Red.HasValue)
        {
            sets.Add("red = @RedNueva");
            p.Add("RedNueva", campos.Red.Value);
        }

        if (campos.Docente.HasValue)
        {
            sets.Add("docente = @DocenteNuevo");
            p.Add("DocenteNuevo", campos.Docente.Value);
        }

        if (campos.FechaInicio.HasValue)
        {
            sets.Add("fecha_inicio = @FechaInicio");
            p.Add(
                "FechaInicio",
                campos.FechaInicio.Value.ToDateTime(
                    TimeOnly.MinValue));
        }

        if (campos.FechaFin != null)
        {
            sets.Add("fecha_fin = @FechaFin");
            p.Add("FechaFin", campos.FechaFin);
        }

        if (campos.ActDestacadas != null)
        {
            sets.Add("act_destacadas = @ActDestacadas");
            p.Add("ActDestacadas", campos.ActDestacadas);
        }

        if (sets.Count == 0)
            return 0;

        var sql = $@"
            UPDATE red_docente
            SET {string.Join(", ", sets)}
            WHERE red = @RedActual
              AND docente = @DocenteActual
              AND activo = 1";

        return await conexion.ExecuteAsync(sql, p);
    }

    public async Task<int> EliminarLogico(
        int red,
        int docente)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            UPDATE red_docente
            SET activo = 0
            WHERE red = @Red
              AND docente = @Docente
              AND activo = 1";

        return await conexion.ExecuteAsync(
            sql,
            new
            {
                Red = red,
                Docente = docente
            });
    }
}
