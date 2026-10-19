using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

// SQL vive en db/version_2.sql. Cada método llama un procedimiento explícito.
public class RepositorioInteresFuturoSqlServer : IRepositorioInteresFuturo
{
    static RepositorioInteresFuturoSqlServer() => FechasSql.Configurar();
    private readonly string cadena;
    public RepositorioInteresFuturoSqlServer(IConfiguration configuracion) => cadena = configuracion.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("Falta la conexión SqlServer.");

    internal static DynamicParameters Parametros(InteresFuturo entidad)
    {
        var p = new DynamicParameters();
        p.Add("Docente", entidad.Docente);
        p.Add("TerminoClave", entidad.TerminoClave);
        return p;
    }

    public async Task<IEnumerable<InteresFuturo>> ObtenerTodos(int limite)
    {
        await using var conexion = new SqlConnection(cadena);
        return await conexion.QueryAsync<InteresFuturo>("dbo.sp_listar_intereses_futuros", new { Limite = limite }, commandType: CommandType.StoredProcedure);
    }

    public async Task<InteresFuturo?> ObtenerPorId(int docente, string terminoClave)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Docente", docente);
        p.Add("TerminoClave", terminoClave);
        return await conexion.QuerySingleOrDefaultAsync<InteresFuturo>("dbo.sp_consultar_intereses_futuros", p, commandType: CommandType.StoredProcedure);
    }

    public async Task Crear(InteresFuturo interes)
    {
        await using var conexion = new SqlConnection(cadena);
        var guardada = await conexion.QuerySingleAsync<InteresFuturo>("dbo.sp_crear_intereses_futuros", Parametros(interes), commandType: CommandType.StoredProcedure);
        interes.Docente = guardada.Docente;
        interes.TerminoClave = guardada.TerminoClave;
    }

    public async Task<int> Reemplazar(int docenteActual, string terminoActual, InteresFuturo interes)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = Parametros(interes);
        p.Add("OriginalDocente", docenteActual);
        p.Add("OriginalTerminoClave", terminoActual);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_intereses_futuros", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> ActualizarParcial(int docenteActual, string terminoActual, InteresFuturoCampos campos)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Docente", campos.Docente);
        p.Add("TerminoClave", campos.TerminoClave);
        p.Add("Parcial", true);
        p.Add("OriginalDocente", docenteActual);
        p.Add("OriginalTerminoClave", terminoActual);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_intereses_futuros", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> EliminarLogico(int docente, string terminoClave)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Docente", docente);
        p.Add("TerminoClave", terminoClave);
        return await conexion.QuerySingleAsync<int>("dbo.sp_eliminar_intereses_futuros", p, commandType: CommandType.StoredProcedure);
    }
}
