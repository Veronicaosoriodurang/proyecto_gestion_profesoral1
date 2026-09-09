using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioReconocimientoSqlServer
    : IRepositorioReconocimiento
{
    private readonly string _cadenaConexion;

    public RepositorioReconocimientoSqlServer(
        IConfiguration configuracion)
    {
        _cadenaConexion =
            configuracion.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'SqlServer'.");
    }

    private IDbConnection CrearConexion() =>
        new SqlConnection(_cadenaConexion);

    public async Task<IEnumerable<Reconocimiento>> ObtenerTodos(
        int limite)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            SELECT TOP (@Limite)
                id AS Id,
                tipo AS Tipo,
                fecha AS Fecha,
                institucion AS Institucion,
                nombre AS Nombre,
                ambito AS Ambito,
                docente AS Docente
            FROM reconocimiento
            WHERE activo = 1
            ORDER BY id";

        return await conexion.QueryAsync<Reconocimiento>(
            sql,
            new { Limite = limite });
    }

    public async Task<Reconocimiento?> ObtenerPorId(int id)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            SELECT
                id AS Id,
                tipo AS Tipo,
                fecha AS Fecha,
                institucion AS Institucion,
                nombre AS Nombre,
                ambito AS Ambito,
                docente AS Docente
            FROM reconocimiento
            WHERE id = @Id
              AND activo = 1";

        return await conexion.QueryFirstOrDefaultAsync<Reconocimiento>(
            sql,
            new { Id = id });
    }

    public async Task<int> Crear(Reconocimiento reconocimiento)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            INSERT INTO reconocimiento
                (tipo, fecha, institucion, nombre, ambito, docente, activo)
            OUTPUT INSERTED.id
            VALUES
                (@Tipo, @Fecha, @Institucion, @Nombre, @Ambito, @Docente, 1)";

        return await conexion.ExecuteScalarAsync<int>(
            sql,
            new
            {
                reconocimiento.Tipo,
                Fecha = reconocimiento.Fecha.ToDateTime(TimeOnly.MinValue),
                reconocimiento.Institucion,
                reconocimiento.Nombre,
                reconocimiento.Ambito,
                reconocimiento.Docente
            });
    }

    public async Task<int> Reemplazar(
        int id,
        Reconocimiento reconocimiento)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            UPDATE reconocimiento
            SET tipo = @Tipo,
                fecha = @Fecha,
                institucion = @Institucion,
                nombre = @Nombre,
                ambito = @Ambito,
                docente = @Docente
            WHERE id = @Id
              AND activo = 1";

        return await conexion.ExecuteAsync(sql, new
        {
            Id = id,
            reconocimiento.Tipo,
            Fecha = reconocimiento.Fecha.ToDateTime(TimeOnly.MinValue),
            reconocimiento.Institucion,
            reconocimiento.Nombre,
            reconocimiento.Ambito,
            reconocimiento.Docente
        });
    }

    public async Task<int> ActualizarParcial(
        int id,
        ReconocimientoCampos campos)
    {
        using var conexion = CrearConexion();

        var sets = new List<string>();
        var p = new DynamicParameters();

        p.Add("Id", id);

        if (campos.Tipo != null)
        {
            sets.Add("tipo = @Tipo");
            p.Add("Tipo", campos.Tipo);
        }

        if (campos.Fecha.HasValue)
        {
            sets.Add("fecha = @Fecha");
            p.Add(
                "Fecha",
                campos.Fecha.Value.ToDateTime(TimeOnly.MinValue));
        }

        if (campos.Institucion != null)
        {
            sets.Add("institucion = @Institucion");
            p.Add("Institucion", campos.Institucion);
        }

        if (campos.Nombre != null)
        {
            sets.Add("nombre = @Nombre");
            p.Add("Nombre", campos.Nombre);
        }

        if (campos.Ambito != null)
        {
            sets.Add("ambito = @Ambito");
            p.Add("Ambito", campos.Ambito);
        }

        if (campos.Docente.HasValue)
        {
            sets.Add("docente = @Docente");
            p.Add("Docente", campos.Docente.Value);
        }

        if (sets.Count == 0)
            return 0;

        var sql = $@"
            UPDATE reconocimiento
            SET {string.Join(", ", sets)}
            WHERE id = @Id
              AND activo = 1";

        return await conexion.ExecuteAsync(sql, p);
    }

    public async Task<int> EliminarLogico(int id)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            UPDATE reconocimiento
            SET activo = 0
            WHERE id = @Id
              AND activo = 1";

        return await conexion.ExecuteAsync(
            sql,
            new { Id = id });
    }
}
