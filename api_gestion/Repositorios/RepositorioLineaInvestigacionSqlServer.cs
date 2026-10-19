using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

// SQL vive en db/version_2.sql. Cada método llama un procedimiento explícito.
public class RepositorioLineaInvestigacionSqlServer : IRepositorioLineaInvestigacion
{
    static RepositorioLineaInvestigacionSqlServer() => FechasSql.Configurar();
    private readonly string cadena;
    public RepositorioLineaInvestigacionSqlServer(IConfiguration configuracion) => cadena = configuracion.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("Falta la conexión SqlServer.");

    internal static DynamicParameters Parametros(LineaInvestigacion entidad)
    {
        var p = new DynamicParameters();
        p.Add("Nombre", entidad.Nombre);
        p.Add("Descripcion", entidad.Descripcion);
        return p;
    }

    public async Task<IEnumerable<LineaInvestigacion>> ObtenerTodos(int limite)
    {
        await using var conexion = new SqlConnection(cadena);
        return await conexion.QueryAsync<LineaInvestigacion>("dbo.sp_listar_linea_investigacion", new { Limite = limite }, commandType: CommandType.StoredProcedure);
    }

    public async Task<LineaInvestigacion?> ObtenerPorId(int id)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Id", id);
        return await conexion.QuerySingleOrDefaultAsync<LineaInvestigacion>("dbo.sp_consultar_linea_investigacion", p, commandType: CommandType.StoredProcedure);
    }

    public async Task Crear(LineaInvestigacion linea)
    {
        await using var conexion = new SqlConnection(cadena);
        var guardada = await conexion.QuerySingleAsync<LineaInvestigacion>("dbo.sp_crear_linea_investigacion", Parametros(linea), commandType: CommandType.StoredProcedure);
        linea.Id = guardada.Id;
        linea.Nombre = guardada.Nombre;
        linea.Descripcion = guardada.Descripcion;
    }

    public async Task<int> Reemplazar(LineaInvestigacion linea)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = Parametros(linea);
        p.Add("OriginalId", linea.Id);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_linea_investigacion", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> ActualizarParcial(int id, LineaInvestigacionCampos campos)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Nombre", campos.Nombre);
        p.Add("Descripcion", campos.Descripcion);
        p.Add("Parcial", true);
        p.Add("OriginalId", id);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_linea_investigacion", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> EliminarLogico(int id)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Id", id);
        return await conexion.QuerySingleAsync<int>("dbo.sp_eliminar_linea_investigacion", p, commandType: CommandType.StoredProcedure);
    }
}
