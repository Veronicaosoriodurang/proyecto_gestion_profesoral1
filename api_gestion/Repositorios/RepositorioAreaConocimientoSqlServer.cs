using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

// SQL vive en db/version_2.sql. Cada método llama un procedimiento explícito.
public class RepositorioAreaConocimientoSqlServer : IRepositorioAreaConocimiento
{
    static RepositorioAreaConocimientoSqlServer() => FechasSql.Configurar();
    private readonly string cadena;
    public RepositorioAreaConocimientoSqlServer(IConfiguration configuracion) => cadena = configuracion.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("Falta la conexión SqlServer.");

    internal static DynamicParameters Parametros(AreaConocimiento entidad)
    {
        var p = new DynamicParameters();
        p.Add("Id", entidad.Id);
        p.Add("GranArea", entidad.GranArea);
        p.Add("Area", entidad.Area);
        p.Add("Disciplina", entidad.Disciplina);
        return p;
    }

    public async Task<IEnumerable<AreaConocimiento>> ObtenerTodos(int limite)
    {
        await using var conexion = new SqlConnection(cadena);
        return await conexion.QueryAsync<AreaConocimiento>("dbo.sp_listar_area_conocimiento", new { Limite = limite }, commandType: CommandType.StoredProcedure);
    }

    public async Task<AreaConocimiento?> ObtenerPorId(string id)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Id", id);
        return await conexion.QuerySingleOrDefaultAsync<AreaConocimiento>("dbo.sp_consultar_area_conocimiento", p, commandType: CommandType.StoredProcedure);
    }

    public async Task Crear(AreaConocimiento areaConocimiento)
    {
        await using var conexion = new SqlConnection(cadena);
        var guardada = await conexion.QuerySingleAsync<AreaConocimiento>("dbo.sp_crear_area_conocimiento", Parametros(areaConocimiento), commandType: CommandType.StoredProcedure);
        areaConocimiento.Id = guardada.Id;
        areaConocimiento.GranArea = guardada.GranArea;
        areaConocimiento.Area = guardada.Area;
        areaConocimiento.Disciplina = guardada.Disciplina;
    }

    public async Task<int> Reemplazar(AreaConocimiento areaConocimiento)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = Parametros(areaConocimiento);
        p.Add("OriginalId", areaConocimiento.Id);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_area_conocimiento", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> ActualizarParcial(string id, AreaConocimientoCampos campos)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("GranArea", campos.GranArea);
        p.Add("Area", campos.Area);
        p.Add("Disciplina", campos.Disciplina);
        p.Add("Id", id);
        p.Add("Parcial", true);
        p.Add("OriginalId", id);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_area_conocimiento", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> EliminarLogico(string id)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Id", id);
        return await conexion.QuerySingleAsync<int>("dbo.sp_eliminar_area_conocimiento", p, commandType: CommandType.StoredProcedure);
    }
}
