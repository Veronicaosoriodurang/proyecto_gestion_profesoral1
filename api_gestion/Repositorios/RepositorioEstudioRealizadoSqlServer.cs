using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

/// <summary>
/// Capa de datos SQL Server para estudios_realizados.
/// Todas las consultas visibles filtran activo = 1.
/// </summary>
public class RepositorioEstudioRealizadoSqlServer : IRepositorioEstudioRealizado
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
                _ => throw new DataException($"No se puede convertir {value.GetType()} a DateOnly.")
            };
        }
    }

    static RepositorioEstudioRealizadoSqlServer()
    {
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
    }

    private readonly string _cadenaConexion;

    public RepositorioEstudioRealizadoSqlServer(IConfiguration configuracion)
    {
        _cadenaConexion = configuracion.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'SqlServer'.");
    }

    private IDbConnection CrearConexion() =>
        new SqlConnection(_cadenaConexion);

    private const string COLUMNAS = @"
        id AS Id,
        titulo AS Titulo,
        universidad AS Universidad,
        fecha AS Fecha,
        tipo AS Tipo,
        ciudad AS Ciudad,
        docente AS Docente,
        ins_acreditada AS InsAcreditada,
        metodologia AS Metodologia,
        perfil_egresado AS PerfilEgresado,
        pais AS Pais";

    public async Task<IEnumerable<EstudioRealizado>> ObtenerTodos(int limite)
    {
        using var conexion = CrearConexion();

        var sql = $@"
            SELECT TOP (@Limite) {COLUMNAS}
            FROM estudios_realizados
            WHERE activo = 1
            ORDER BY id ASC";

        return await conexion.QueryAsync<EstudioRealizado>(
            sql,
            new { Limite = limite });
    }

    public async Task<EstudioRealizado?> ObtenerPorId(int id)
    {
        using var conexion = CrearConexion();

        var sql = $@"
            SELECT {COLUMNAS}
            FROM estudios_realizados
            WHERE id = @Id
              AND activo = 1";

        return await conexion.QueryFirstOrDefaultAsync<EstudioRealizado>(
            sql,
            new { Id = id });
    }

    public async Task Crear(EstudioRealizado estudio)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            INSERT INTO estudios_realizados
            (
                id,
                titulo,
                universidad,
                fecha,
                tipo,
                ciudad,
                docente,
                ins_acreditada,
                metodologia,
                perfil_egresado,
                pais,
                activo
            )
            VALUES
            (
                @Id,
                @Titulo,
                @Universidad,
                @Fecha,
                @Tipo,
                @Ciudad,
                @Docente,
                @InsAcreditada,
                @Metodologia,
                @PerfilEgresado,
                @Pais,
                1
            )";

        await conexion.ExecuteAsync(sql, new
        {
            estudio.Id,
            estudio.Titulo,
            estudio.Universidad,
            Fecha = estudio.Fecha.ToDateTime(TimeOnly.MinValue),
            estudio.Tipo,
            estudio.Ciudad,
            estudio.Docente,
            estudio.InsAcreditada,
            estudio.Metodologia,
            estudio.PerfilEgresado,
            estudio.Pais
        });
    }

    public async Task<int> Reemplazar(EstudioRealizado estudio)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            UPDATE estudios_realizados
            SET titulo = @Titulo,
                universidad = @Universidad,
                fecha = @Fecha,
                tipo = @Tipo,
                ciudad = @Ciudad,
                docente = @Docente,
                ins_acreditada = @InsAcreditada,
                metodologia = @Metodologia,
                perfil_egresado = @PerfilEgresado,
                pais = @Pais
            WHERE id = @Id
              AND activo = 1";

        return await conexion.ExecuteAsync(sql, new
        {
            estudio.Id,
            estudio.Titulo,
            estudio.Universidad,
            Fecha = estudio.Fecha.ToDateTime(TimeOnly.MinValue),
            estudio.Tipo,
            estudio.Ciudad,
            estudio.Docente,
            estudio.InsAcreditada,
            estudio.Metodologia,
            estudio.PerfilEgresado,
            estudio.Pais
        });
    }

    public async Task<int> ActualizarParcial(
        int id,
        EstudioRealizadoCampos campos)
    {
        using var conexion = CrearConexion();

        var asignaciones = new List<string>();
        var parametros = new DynamicParameters();

        parametros.Add("Id", id);

        void Agregar(string columna, string parametro, object? valor)
        {
            if (valor == null)
                return;

            asignaciones.Add($"{columna} = @{parametro}");
            parametros.Add(parametro, valor);
        }

        Agregar("titulo", "Titulo", campos.Titulo);
        Agregar("universidad", "Universidad", campos.Universidad);

        if (campos.Fecha.HasValue)
        {
            Agregar(
                "fecha",
                "Fecha",
                campos.Fecha.Value.ToDateTime(TimeOnly.MinValue));
        }

        Agregar("tipo", "Tipo", campos.Tipo);
        Agregar("ciudad", "Ciudad", campos.Ciudad);
        Agregar("docente", "Docente", campos.Docente);
        Agregar("ins_acreditada", "InsAcreditada", campos.InsAcreditada);
        Agregar("metodologia", "Metodologia", campos.Metodologia);
        Agregar("perfil_egresado", "PerfilEgresado", campos.PerfilEgresado);
        Agregar("pais", "Pais", campos.Pais);

        if (asignaciones.Count == 0)
            return 0;

        var sql = $@"
            UPDATE estudios_realizados
            SET {string.Join(", ", asignaciones)}
            WHERE id = @Id
              AND activo = 1";

        return await conexion.ExecuteAsync(sql, parametros);
    }

    public async Task<int> EliminarLogico(int id)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            UPDATE estudios_realizados
            SET activo = 0
            WHERE id = @Id
              AND activo = 1";

        return await conexion.ExecuteAsync(sql, new { Id = id });
    }
}
