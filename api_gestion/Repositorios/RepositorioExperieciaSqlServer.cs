using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

// SQL vive en db/version_2.sql. Cada método llama un procedimiento explícito.
public class RepositorioExperieciaSqlServer : IRepositorioExperiecia
{
    static RepositorioExperieciaSqlServer() => FechasSql.Configurar();
    private readonly string cadena;
    public RepositorioExperieciaSqlServer(IConfiguration configuracion) => cadena = configuracion.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("Falta la conexión SqlServer.");

    internal static DynamicParameters Parametros(Experiecia entidad)
    {
        var p = new DynamicParameters();
        p.Add("NombreCargo", entidad.NombreCargo);
        p.Add("Institucion", entidad.Institucion);
        p.Add("Tipo", entidad.Tipo);
        p.Add("FechaInicio", entidad.FechaInicio.ToDateTime(TimeOnly.MinValue));
        p.Add("FechaFin", entidad.FechaFin?.ToDateTime(TimeOnly.MinValue));
        p.Add("Docente", entidad.Docente);
        return p;
    }

    public async Task<IEnumerable<Experiecia>> ObtenerTodos(int limite)
    {
        await using var conexion = new SqlConnection(cadena);
        return await conexion.QueryAsync<Experiecia>("dbo.sp_listar_experiecia", new { Limite = limite }, commandType: CommandType.StoredProcedure);
    }

    public async Task<Experiecia?> ObtenerPorId(int id)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Id", id);
        return await conexion.QuerySingleOrDefaultAsync<Experiecia>("dbo.sp_consultar_experiecia", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> Crear(Experiecia experiencia)
    {
        await using var conexion = new SqlConnection(cadena);
        var guardada = await conexion.QuerySingleAsync<Experiecia>("dbo.sp_crear_experiecia", Parametros(experiencia), commandType: CommandType.StoredProcedure);
        experiencia.Id = guardada.Id;
        experiencia.NombreCargo = guardada.NombreCargo;
        experiencia.Institucion = guardada.Institucion;
        experiencia.Tipo = guardada.Tipo;
        experiencia.FechaInicio = guardada.FechaInicio;
        experiencia.FechaFin = guardada.FechaFin;
        experiencia.Docente = guardada.Docente;
        return guardada.Id;
    }

    public async Task<int> Reemplazar(int id, Experiecia experiencia)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = Parametros(experiencia);
        p.Add("OriginalId", id);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_experiecia", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> ActualizarParcial(int id, ExperieciaCampos campos)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("NombreCargo", campos.NombreCargo);
        p.Add("Institucion", campos.Institucion);
        p.Add("Tipo", campos.Tipo);
        p.Add("FechaInicio", campos.FechaInicio?.ToDateTime(TimeOnly.MinValue));
        p.Add("FechaFin", campos.FechaFin?.ToDateTime(TimeOnly.MinValue));
        p.Add("Docente", campos.Docente);
        p.Add("Parcial", true);
        p.Add("OriginalId", id);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_experiecia", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> EliminarLogico(int id)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Id", id);
        return await conexion.QuerySingleAsync<int>("dbo.sp_eliminar_experiecia", p, commandType: CommandType.StoredProcedure);
    }
}
