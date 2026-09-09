using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

/// <summary>
/// Capa de datos SQL Server para docente_departamento.
/// Todas las consultas visibles filtran activo = 1.
/// </summary>
public class RepositorioDocenteDepartamentoSqlServer : IRepositorioDocenteDepartamento
{
    private sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
    {
        public override void SetValue(IDbDataParameter parameter, DateOnly value)
        {
            parameter.Value = value.ToDateTime(TimeOnly.MinValue);
            parameter.DbType = DbType.Date;
        }

        public override DateOnly Parse(object value)
        {
            return value switch
            {
                DateTime fecha => DateOnly.FromDateTime(fecha),
                DateOnly fecha => fecha,
                _ => throw new DataException(
                    $"No se puede convertir {value.GetType()} a DateOnly.")
            };
        }
    }

    static RepositorioDocenteDepartamentoSqlServer()
    {
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
    }

    private readonly string _cadenaConexion;

    public RepositorioDocenteDepartamentoSqlServer(IConfiguration configuracion)
    {
        _cadenaConexion = configuracion.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'SqlServer'.");
    }

    private IDbConnection CrearConexion() =>
        new SqlConnection(_cadenaConexion);

    private const string COLUMNAS = @"
        docente AS Docente,
        departamento AS Departamento,
        dedicacion AS Dedicacion,
        modalidad AS Modalidad,
        fecha_ingreso AS FechaIngreso,
        fecha_salida AS FechaSalida";

    public async Task<IEnumerable<DocenteDepartamento>> ObtenerTodos(int limite)
    {
        using var conexion = CrearConexion();

        var sql = $@"
            SELECT TOP (@Limite) {COLUMNAS}
            FROM docente_departamento
            WHERE activo = 1
            ORDER BY docente ASC, departamento ASC";

        return await conexion.QueryAsync<DocenteDepartamento>(
            sql,
            new { Limite = limite });
    }

    public async Task<DocenteDepartamento?> ObtenerPorId(
        int docente,
        int departamento)
    {
        using var conexion = CrearConexion();

        var sql = $@"
            SELECT {COLUMNAS}
            FROM docente_departamento
            WHERE docente = @Docente
              AND departamento = @Departamento
              AND activo = 1";

        return await conexion.QueryFirstOrDefaultAsync<DocenteDepartamento>(
            sql,
            new
            {
                Docente = docente,
                Departamento = departamento
            });
    }

    public async Task Crear(DocenteDepartamento relacion)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            INSERT INTO docente_departamento
            (
                docente,
                departamento,
                dedicacion,
                modalidad,
                fecha_ingreso,
                fecha_salida,
                activo
            )
            VALUES
            (
                @Docente,
                @Departamento,
                @Dedicacion,
                @Modalidad,
                @FechaIngreso,
                @FechaSalida,
                1
            )";

        await conexion.ExecuteAsync(sql, new
        {
            relacion.Docente,
            relacion.Departamento,
            relacion.Dedicacion,
            relacion.Modalidad,
            FechaIngreso =
                relacion.FechaIngreso.ToDateTime(TimeOnly.MinValue),
            FechaSalida =
                relacion.FechaSalida.HasValue
                    ? relacion.FechaSalida.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null
        });
    }

    public async Task<int> Reemplazar(DocenteDepartamento relacion)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            UPDATE docente_departamento
            SET dedicacion = @Dedicacion,
                modalidad = @Modalidad,
                fecha_ingreso = @FechaIngreso,
                fecha_salida = @FechaSalida
            WHERE docente = @Docente
              AND departamento = @Departamento
              AND activo = 1";

        return await conexion.ExecuteAsync(sql, new
        {
            relacion.Docente,
            relacion.Departamento,
            relacion.Dedicacion,
            relacion.Modalidad,
            FechaIngreso =
                relacion.FechaIngreso.ToDateTime(TimeOnly.MinValue),
            FechaSalida =
                relacion.FechaSalida.HasValue
                    ? relacion.FechaSalida.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null
        });
    }

    public async Task<int> ActualizarParcial(
        int docente,
        int departamento,
        DocenteDepartamentoCampos campos)
    {
        using var conexion = CrearConexion();

        var asignaciones = new List<string>();
        var parametros = new DynamicParameters();

        parametros.Add("Docente", docente);
        parametros.Add("Departamento", departamento);

        void Agregar(string columna, string parametro, object? valor)
        {
            if (valor == null)
                return;

            asignaciones.Add($"{columna} = @{parametro}");
            parametros.Add(parametro, valor);
        }

        Agregar("dedicacion", "Dedicacion", campos.Dedicacion);
        Agregar("modalidad", "Modalidad", campos.Modalidad);

        if (campos.FechaIngreso.HasValue)
        {
            Agregar(
                "fecha_ingreso",
                "FechaIngreso",
                campos.FechaIngreso.Value.ToDateTime(TimeOnly.MinValue));
        }

        if (campos.FechaSalida.HasValue)
        {
            Agregar(
                "fecha_salida",
                "FechaSalida",
                campos.FechaSalida.Value.ToDateTime(TimeOnly.MinValue));
        }

        if (asignaciones.Count == 0)
            return 0;

        var sql = $@"
            UPDATE docente_departamento
            SET {string.Join(", ", asignaciones)}
            WHERE docente = @Docente
              AND departamento = @Departamento
              AND activo = 1";

        return await conexion.ExecuteAsync(sql, parametros);
    }

    public async Task<int> EliminarLogico(int docente, int departamento)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            UPDATE docente_departamento
            SET activo = 0
            WHERE docente = @Docente
              AND departamento = @Departamento
              AND activo = 1";

        return await conexion.ExecuteAsync(sql, new
        {
            Docente = docente,
            Departamento = departamento
        });
    }
}
