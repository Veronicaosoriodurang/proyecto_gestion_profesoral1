using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

// SQL vive en db/version_2.sql. Cada método llama un procedimiento explícito.
public class RepositorioEstudioAcSqlServer : IRepositorioEstudioAc
{
    static RepositorioEstudioAcSqlServer() => FechasSql.Configurar();
    private readonly string cadena;
    public RepositorioEstudioAcSqlServer(IConfiguration configuracion) => cadena = configuracion.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("Falta la conexión SqlServer.");

    internal static DynamicParameters Parametros(EstudioAc entidad)
    {
        var p = new DynamicParameters();
        p.Add("Estudio", entidad.Estudio);
        p.Add("AreaConocimiento", entidad.AreaConocimiento);
        return p;
    }

    public async Task<IEnumerable<EstudioAc>> ObtenerTodos(int limite)
    {
        await using var conexion = new SqlConnection(cadena);
        return await conexion.QueryAsync<EstudioAc>("dbo.sp_listar_estudio_ac", new { Limite = limite }, commandType: CommandType.StoredProcedure);
    }

    public async Task<EstudioAc?> ObtenerPorId(int estudio, string areaConocimiento)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Estudio", estudio);
        p.Add("AreaConocimiento", areaConocimiento);
        return await conexion.QuerySingleOrDefaultAsync<EstudioAc>("dbo.sp_consultar_estudio_ac", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> Crear(EstudioAc entidad)
    {
        await using var conexion = new SqlConnection(cadena);
        var guardada = await conexion.QuerySingleAsync<EstudioAc>("dbo.sp_crear_estudio_ac", Parametros(entidad), commandType: CommandType.StoredProcedure);
        entidad.Estudio = guardada.Estudio;
        entidad.AreaConocimiento = guardada.AreaConocimiento;
        return 1;
    }

    public async Task<int> Reemplazar(int estudioActual, string areaActual, EstudioAc entidad)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = Parametros(entidad);
        p.Add("OriginalEstudio", estudioActual);
        p.Add("OriginalAreaConocimiento", areaActual);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_estudio_ac", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> ActualizarParcial(int estudioActual, string areaActual, EstudioAcCampos campos)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Estudio", campos.Estudio);
        p.Add("AreaConocimiento", campos.AreaConocimiento);
        p.Add("Parcial", true);
        p.Add("OriginalEstudio", estudioActual);
        p.Add("OriginalAreaConocimiento", areaActual);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_estudio_ac", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> EliminarLogico(int estudio, string areaConocimiento)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Estudio", estudio);
        p.Add("AreaConocimiento", areaConocimiento);
        return await conexion.QuerySingleAsync<int>("dbo.sp_eliminar_estudio_ac", p, commandType: CommandType.StoredProcedure);
    }
}
