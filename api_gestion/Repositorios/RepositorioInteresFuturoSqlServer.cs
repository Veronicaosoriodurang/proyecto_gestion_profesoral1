using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioInteresFuturoSqlServer : IRepositorioInteresFuturo
{
    private readonly string _cadenaConexion;

    public RepositorioInteresFuturoSqlServer(IConfiguration configuracion)
    {
        _cadenaConexion =
            configuracion.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'SqlServer'.");
    }

    private IDbConnection CrearConexion() =>
        new SqlConnection(_cadenaConexion);

    public async Task<IEnumerable<InteresFuturo>> ObtenerTodos(int limite)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            SELECT TOP (@Limite)
                docente AS Docente,
                termino_clave AS TerminoClave
            FROM intereses_futuros
            WHERE activo = 1
            ORDER BY docente, termino_clave";

        return await conexion.QueryAsync<InteresFuturo>(
            sql,
            new { Limite = limite });
    }

    public async Task<InteresFuturo?> ObtenerPorId(
        int docente,
        string terminoClave)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            SELECT
                docente AS Docente,
                termino_clave AS TerminoClave
            FROM intereses_futuros
            WHERE docente = @Docente
              AND termino_clave = @TerminoClave
              AND activo = 1";

        return await conexion.QueryFirstOrDefaultAsync<InteresFuturo>(
            sql,
            new
            {
                Docente = docente,
                TerminoClave = terminoClave
            });
    }

    public async Task Crear(InteresFuturo interes)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            INSERT INTO intereses_futuros
                (docente, termino_clave, activo)
            VALUES
                (@Docente, @TerminoClave, 1)";

        await conexion.ExecuteAsync(sql, interes);
    }

    public async Task<int> Reemplazar(
        int docenteActual,
        string terminoActual,
        InteresFuturo interes)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            UPDATE intereses_futuros
            SET docente = @NuevoDocente,
                termino_clave = @NuevoTermino
            WHERE docente = @DocenteActual
              AND termino_clave = @TerminoActual
              AND activo = 1";

        return await conexion.ExecuteAsync(sql, new
        {
            DocenteActual = docenteActual,
            TerminoActual = terminoActual,
            NuevoDocente = interes.Docente,
            NuevoTermino = interes.TerminoClave
        });
    }

    public async Task<int> ActualizarParcial(
        int docenteActual,
        string terminoActual,
        InteresFuturoCampos campos)
    {
        using var conexion = CrearConexion();

        var asignaciones = new List<string>();
        var parametros = new DynamicParameters();

        parametros.Add("DocenteActual", docenteActual);
        parametros.Add("TerminoActual", terminoActual);

        if (campos.Docente.HasValue)
        {
            asignaciones.Add("docente = @NuevoDocente");
            parametros.Add("NuevoDocente", campos.Docente.Value);
        }

        if (campos.TerminoClave != null)
        {
            asignaciones.Add("termino_clave = @NuevoTermino");
            parametros.Add("NuevoTermino", campos.TerminoClave);
        }

        if (asignaciones.Count == 0)
            return 0;

        var sql = $@"
            UPDATE intereses_futuros
            SET {string.Join(", ", asignaciones)}
            WHERE docente = @DocenteActual
              AND termino_clave = @TerminoActual
              AND activo = 1";

        return await conexion.ExecuteAsync(sql, parametros);
    }

    public async Task<int> EliminarLogico(
        int docente,
        string terminoClave)
    {
        using var conexion = CrearConexion();

        const string sql = @"
            UPDATE intereses_futuros
            SET activo = 0
            WHERE docente = @Docente
              AND termino_clave = @TerminoClave
              AND activo = 1";

        return await conexion.ExecuteAsync(sql, new
        {
            Docente = docente,
            TerminoClave = terminoClave
        });
    }
}
