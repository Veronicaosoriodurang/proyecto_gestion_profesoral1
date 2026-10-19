using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

// SQL vive en db/version_2.sql. Cada método llama un procedimiento explícito.
public class RepositorioRedSqlServer : IRepositorioRed
{
    static RepositorioRedSqlServer() => FechasSql.Configurar();
    private readonly string cadena;
    public RepositorioRedSqlServer(IConfiguration configuracion) => cadena = configuracion.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("Falta la conexión SqlServer.");

    internal static DynamicParameters Parametros(Red entidad)
    {
        var p = new DynamicParameters();
        p.Add("Idr", entidad.Idr);
        p.Add("Nombre", entidad.Nombre);
        p.Add("Url", entidad.Url);
        p.Add("Pais", entidad.Pais);
        return p;
    }

    public async Task<IEnumerable<Red>> ObtenerTodos(int limite)
    {
        await using var conexion = new SqlConnection(cadena);
        return await conexion.QueryAsync<Red>("dbo.sp_listar_red", new { Limite = limite }, commandType: CommandType.StoredProcedure);
    }

    public async Task<Red?> ObtenerPorId(int idr)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Idr", idr);
        return await conexion.QuerySingleOrDefaultAsync<Red>("dbo.sp_consultar_red", p, commandType: CommandType.StoredProcedure);
    }

    public async Task Crear(Red red)
    {
        await using var conexion = new SqlConnection(cadena);
        var guardada = await conexion.QuerySingleAsync<Red>("dbo.sp_crear_red", Parametros(red), commandType: CommandType.StoredProcedure);
        red.Idr = guardada.Idr;
        red.Nombre = guardada.Nombre;
        red.Url = guardada.Url;
        red.Pais = guardada.Pais;
    }

    public async Task<int> Reemplazar(Red red)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = Parametros(red);
        p.Add("OriginalIdr", red.Idr);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_red", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> ActualizarParcial(int idr, RedCampos campos)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Nombre", campos.Nombre);
        p.Add("Url", campos.Url);
        p.Add("Pais", campos.Pais);
        p.Add("Idr", idr);
        p.Add("Parcial", true);
        p.Add("OriginalIdr", idr);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_red", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> EliminarLogico(int idr)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Idr", idr);
        return await conexion.QuerySingleAsync<int>("dbo.sp_eliminar_red", p, commandType: CommandType.StoredProcedure);
    }
}
