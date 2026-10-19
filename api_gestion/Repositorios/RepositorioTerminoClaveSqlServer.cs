using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

// SQL vive en db/version_2.sql. Cada método llama un procedimiento explícito.
public class RepositorioTerminoClaveSqlServer : IRepositorioTerminoClave
{
    static RepositorioTerminoClaveSqlServer() => FechasSql.Configurar();
    private readonly string cadena;
    public RepositorioTerminoClaveSqlServer(IConfiguration configuracion) => cadena = configuracion.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("Falta la conexión SqlServer.");

    internal static DynamicParameters Parametros(TerminoClave entidad)
    {
        var p = new DynamicParameters();
        p.Add("Termino", entidad.Termino);
        p.Add("TerminoIngles", entidad.TerminoIngles);
        return p;
    }

    public async Task<IEnumerable<TerminoClave>> ObtenerTodos(int limite)
    {
        await using var conexion = new SqlConnection(cadena);
        return await conexion.QueryAsync<TerminoClave>("dbo.sp_listar_termino_clave", new { Limite = limite }, commandType: CommandType.StoredProcedure);
    }

    public async Task<TerminoClave?> ObtenerPorTermino(string termino)
    {
        await using var conexion = new SqlConnection(cadena);
        return await conexion.QuerySingleOrDefaultAsync<TerminoClave>("dbo.sp_consultar_termino_clave", new { Termino = termino }, commandType: CommandType.StoredProcedure);
    }

    public async Task Crear(TerminoClave terminoClave)
    {
        await using var conexion = new SqlConnection(cadena);
        var guardada = await conexion.QuerySingleAsync<TerminoClave>("dbo.sp_crear_termino_clave", Parametros(terminoClave), commandType: CommandType.StoredProcedure);
        terminoClave.Termino = guardada.Termino;
        terminoClave.TerminoIngles = guardada.TerminoIngles;
    }

    public async Task<int> Reemplazar(TerminoClave terminoClave)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = Parametros(terminoClave);
        p.Add("OriginalTermino", terminoClave.Termino);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_termino_clave", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> ActualizarParcial(string termino, TerminoClaveCampos campos)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("TerminoIngles", campos.TerminoIngles);
        p.Add("Termino", termino);
        p.Add("Parcial", true);
        p.Add("OriginalTermino", termino);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_termino_clave", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> EliminarLogico(string termino)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Termino", termino);
        return await conexion.QuerySingleAsync<int>("dbo.sp_eliminar_termino_clave", p, commandType: CommandType.StoredProcedure);
    }
}
