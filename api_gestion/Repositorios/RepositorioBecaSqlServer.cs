using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

// SQL vive en db/version_2.sql. Cada método llama un procedimiento explícito.
public class RepositorioBecaSqlServer : IRepositorioBeca
{
    static RepositorioBecaSqlServer() => FechasSql.Configurar();
    private readonly string cadena;
    public RepositorioBecaSqlServer(IConfiguration configuracion) => cadena = configuracion.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("Falta la conexión SqlServer.");

    internal static DynamicParameters Parametros(Beca entidad)
    {
        var p = new DynamicParameters();
        p.Add("Estudios", entidad.Estudios);
        p.Add("Tipo", entidad.Tipo);
        p.Add("Institucion", entidad.Institucion);
        p.Add("FechaInicio", entidad.FechaInicio.ToDateTime(TimeOnly.MinValue));
        p.Add("FechaFin", entidad.FechaFin?.ToDateTime(TimeOnly.MinValue));
        return p;
    }

    public async Task<IEnumerable<Beca>> ObtenerTodos(int limite)
    {
        await using var conexion = new SqlConnection(cadena);
        return await conexion.QueryAsync<Beca>("dbo.sp_listar_beca", new { Limite = limite }, commandType: CommandType.StoredProcedure);
    }

    public async Task<Beca?> ObtenerPorId(int estudios)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Estudios", estudios);
        return await conexion.QuerySingleOrDefaultAsync<Beca>("dbo.sp_consultar_beca", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> Crear(Beca entidad)
    {
        await using var conexion = new SqlConnection(cadena);
        var guardada = await conexion.QuerySingleAsync<Beca>("dbo.sp_crear_beca", Parametros(entidad), commandType: CommandType.StoredProcedure);
        entidad.Estudios = guardada.Estudios;
        entidad.Tipo = guardada.Tipo;
        entidad.Institucion = guardada.Institucion;
        entidad.FechaInicio = guardada.FechaInicio;
        entidad.FechaFin = guardada.FechaFin;
        return 1;
    }

    public async Task<int> Reemplazar(int estudiosActual, Beca entidad)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = Parametros(entidad);
        p.Add("OriginalEstudios", estudiosActual);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_beca", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> ActualizarParcial(int estudiosActual, BecaCampos campos)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Estudios", campos.Estudios);
        p.Add("Tipo", campos.Tipo);
        p.Add("Institucion", campos.Institucion);
        p.Add("FechaInicio", campos.FechaInicio?.ToDateTime(TimeOnly.MinValue));
        p.Add("FechaFin", campos.FechaFin?.ToDateTime(TimeOnly.MinValue));
        p.Add("Parcial", true);
        p.Add("OriginalEstudios", estudiosActual);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_beca", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> EliminarLogico(int estudios)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Estudios", estudios);
        return await conexion.QuerySingleAsync<int>("dbo.sp_eliminar_beca", p, commandType: CommandType.StoredProcedure);
    }
}
