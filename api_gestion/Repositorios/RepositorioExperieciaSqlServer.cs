using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioExperieciaSqlServer
    : IRepositorioExperiecia
{
    private readonly string _cadenaConexion;

    public RepositorioExperieciaSqlServer(
        IConfiguration configuracion)
    {
        _cadenaConexion =
            configuracion.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'SqlServer'.");
    }

    private IDbConnection CrearConexion() =>
        new SqlConnection(_cadenaConexion);

    public async Task<IEnumerable<Experiecia>> ObtenerTodos(
        int limite)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            SELECT TOP (@Limite)
                id AS Id,
                nombre_cargo AS NombreCargo,
                institucion AS Institucion,
                tipo AS Tipo,
                fecha_inicio AS FechaInicio,
                fecha_fin AS FechaFin,
                docente AS Docente
            FROM experiecia
            WHERE activo = 1
            ORDER BY id";

        return await conexion.QueryAsync<Experiecia>(
            sql,
            new { Limite = limite });
    }

    public async Task<Experiecia?> ObtenerPorId(int id)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            SELECT
                id AS Id,
                nombre_cargo AS NombreCargo,
                institucion AS Institucion,
                tipo AS Tipo,
                fecha_inicio AS FechaInicio,
                fecha_fin AS FechaFin,
                docente AS Docente
            FROM experiecia
            WHERE id = @Id
              AND activo = 1";

        return await conexion.QueryFirstOrDefaultAsync<Experiecia>(
            sql,
            new { Id = id });
    }

    public async Task<int> Crear(Experiecia experiencia)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            INSERT INTO experiecia
                (
                    nombre_cargo,
                    institucion,
                    tipo,
                    fecha_inicio,
                    fecha_fin,
                    docente,
                    activo
                )
            OUTPUT INSERTED.id
            VALUES
                (
                    @NombreCargo,
                    @Institucion,
                    @Tipo,
                    @FechaInicio,
                    @FechaFin,
                    @Docente,
                    1
                )";

        return await conexion.ExecuteScalarAsync<int>(
            sql,
            new
            {
                experiencia.NombreCargo,
                experiencia.Institucion,
                experiencia.Tipo,
                FechaInicio =
                    experiencia.FechaInicio.ToDateTime(
                        TimeOnly.MinValue),
                FechaFin =
                    experiencia.FechaFin?.ToDateTime(
                        TimeOnly.MinValue),
                experiencia.Docente
            });
    }

    public async Task<int> Reemplazar(
        int id,
        Experiecia experiencia)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            UPDATE experiecia
            SET nombre_cargo = @NombreCargo,
                institucion = @Institucion,
                tipo = @Tipo,
                fecha_inicio = @FechaInicio,
                fecha_fin = @FechaFin,
                docente = @Docente
            WHERE id = @Id
              AND activo = 1";

        return await conexion.ExecuteAsync(
            sql,
            new
            {
                Id = id,
                experiencia.NombreCargo,
                experiencia.Institucion,
                experiencia.Tipo,
                FechaInicio =
                    experiencia.FechaInicio.ToDateTime(
                        TimeOnly.MinValue),
                FechaFin =
                    experiencia.FechaFin?.ToDateTime(
                        TimeOnly.MinValue),
                experiencia.Docente
            });
    }

    public async Task<int> ActualizarParcial(
        int id,
        ExperieciaCampos campos)
    {
        using var conexion = CrearConexion();

        var sets = new List<string>();
        var p = new DynamicParameters();

        p.Add("Id", id);

        if (campos.NombreCargo != null)
        {
            sets.Add("nombre_cargo = @NombreCargo");
            p.Add("NombreCargo", campos.NombreCargo);
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

        if (campos.FechaInicio.HasValue)
        {
            sets.Add("fecha_inicio = @FechaInicio");
            p.Add(
                "FechaInicio",
                campos.FechaInicio.Value.ToDateTime(
                    TimeOnly.MinValue));
        }

        if (campos.FechaFin.HasValue)
        {
            sets.Add("fecha_fin = @FechaFin");
            p.Add(
                "FechaFin",
                campos.FechaFin.Value.ToDateTime(
                    TimeOnly.MinValue));
        }

        if (campos.Docente.HasValue)
        {
            sets.Add("docente = @Docente");
            p.Add("Docente", campos.Docente.Value);
        }

        if (sets.Count == 0)
            return 0;

        var sql = $@"
            UPDATE experiecia
            SET {string.Join(", ", sets)}
            WHERE id = @Id
              AND activo = 1";

        return await conexion.ExecuteAsync(sql, p);
    }

    public async Task<int> EliminarLogico(int id)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            UPDATE experiecia
            SET activo = 0
            WHERE id = @Id
              AND activo = 1";

        return await conexion.ExecuteAsync(
            sql,
            new { Id = id });
    }
}
