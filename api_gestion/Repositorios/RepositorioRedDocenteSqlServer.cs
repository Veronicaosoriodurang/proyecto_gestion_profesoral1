using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

// SQL vive en db/version_2.sql. Cada método llama un procedimiento explícito.
public class RepositorioRedDocenteSqlServer : IRepositorioRedDocente
{
    static RepositorioRedDocenteSqlServer() => FechasSql.Configurar();
    private readonly string cadena;
    public RepositorioRedDocenteSqlServer(IConfiguration configuracion) => cadena = configuracion.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("Falta la conexión SqlServer.");

    internal static DynamicParameters Parametros(RedDocente entidad)
    {
        var p = new DynamicParameters();
        p.Add("Red", entidad.Red);
        p.Add("Docente", entidad.Docente);
        p.Add("FechaInicio", entidad.FechaInicio.ToDateTime(TimeOnly.MinValue));
        p.Add("FechaFin", entidad.FechaFin);
        p.Add("ActDestacadas", entidad.ActDestacadas);
        return p;
    }

    public async Task<IEnumerable<RedDocente>> ObtenerTodos(int limite)
    {
        await using var conexion = new SqlConnection(cadena);
        return await conexion.QueryAsync<RedDocente>("dbo.sp_listar_red_docente", new { Limite = limite }, commandType: CommandType.StoredProcedure);
    }

    public async Task<RedDocente?> ObtenerPorId(int red, int docente)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Red", red);
        p.Add("Docente", docente);
        return await conexion.QuerySingleOrDefaultAsync<RedDocente>("dbo.sp_consultar_red_docente", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> Crear(RedDocente entidad)
    {
        await using var conexion = new SqlConnection(cadena);
        var guardada = await conexion.QuerySingleAsync<RedDocente>("dbo.sp_crear_red_docente", Parametros(entidad), commandType: CommandType.StoredProcedure);
        entidad.Red = guardada.Red;
        entidad.Docente = guardada.Docente;
        entidad.FechaInicio = guardada.FechaInicio;
        entidad.FechaFin = guardada.FechaFin;
        entidad.ActDestacadas = guardada.ActDestacadas;
        return 1;
    }

    public async Task<int> Reemplazar(int redActual, int docenteActual, RedDocente entidad)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = Parametros(entidad);
        p.Add("OriginalRed", redActual);
        p.Add("OriginalDocente", docenteActual);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_red_docente", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> ActualizarParcial(int redActual, int docenteActual, RedDocenteCampos campos)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Red", campos.Red);
        p.Add("Docente", campos.Docente);
        p.Add("FechaInicio", campos.FechaInicio?.ToDateTime(TimeOnly.MinValue));
        p.Add("FechaFin", campos.FechaFin);
        p.Add("ActDestacadas", campos.ActDestacadas);
        p.Add("Parcial", true);
        p.Add("OriginalRed", redActual);
        p.Add("OriginalDocente", docenteActual);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_red_docente", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> EliminarLogico(int red, int docente)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Red", red);
        p.Add("Docente", docente);
        return await conexion.QuerySingleAsync<int>("dbo.sp_eliminar_red_docente", p, commandType: CommandType.StoredProcedure);
    }
}
