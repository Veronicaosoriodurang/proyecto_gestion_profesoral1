using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

// SQL vive en db/version_2.sql. Cada método llama un procedimiento explícito.
public class RepositorioApoyoProfesoralSqlServer : IRepositorioApoyoProfesoral
{
    static RepositorioApoyoProfesoralSqlServer() => FechasSql.Configurar();
    private readonly string cadena;
    public RepositorioApoyoProfesoralSqlServer(IConfiguration configuracion) => cadena = configuracion.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("Falta la conexión SqlServer.");

    internal static DynamicParameters Parametros(ApoyoProfesoral entidad)
    {
        var p = new DynamicParameters();
        p.Add("Estudios", entidad.Estudios);
        p.Add("ConApoyo", entidad.ConApoyo);
        p.Add("Institucion", entidad.Institucion);
        p.Add("Tipo", entidad.Tipo);
        return p;
    }

    public async Task<IEnumerable<ApoyoProfesoral>> ObtenerTodos(int limite)
    {
        await using var conexion = new SqlConnection(cadena);
        return await conexion.QueryAsync<ApoyoProfesoral>("dbo.sp_listar_apoyo_profesoral", new { Limite = limite }, commandType: CommandType.StoredProcedure);
    }

    public async Task<ApoyoProfesoral?> ObtenerPorId(int estudios)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Estudios", estudios);
        return await conexion.QuerySingleOrDefaultAsync<ApoyoProfesoral>("dbo.sp_consultar_apoyo_profesoral", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> Crear(ApoyoProfesoral entidad)
    {
        await using var conexion = new SqlConnection(cadena);
        var guardada = await conexion.QuerySingleAsync<ApoyoProfesoral>("dbo.sp_crear_apoyo_profesoral", Parametros(entidad), commandType: CommandType.StoredProcedure);
        entidad.Estudios = guardada.Estudios;
        entidad.ConApoyo = guardada.ConApoyo;
        entidad.Institucion = guardada.Institucion;
        entidad.Tipo = guardada.Tipo;
        return 1;
    }

    public async Task<int> Reemplazar(int estudiosActual, ApoyoProfesoral entidad)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = Parametros(entidad);
        p.Add("OriginalEstudios", estudiosActual);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_apoyo_profesoral", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> ActualizarParcial(int estudiosActual, ApoyoProfesoralCampos campos)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Estudios", campos.Estudios);
        p.Add("ConApoyo", campos.ConApoyo);
        p.Add("Institucion", campos.Institucion);
        p.Add("Tipo", campos.Tipo);
        p.Add("Parcial", true);
        p.Add("OriginalEstudios", estudiosActual);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_apoyo_profesoral", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> EliminarLogico(int estudios)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Estudios", estudios);
        return await conexion.QuerySingleAsync<int>("dbo.sp_eliminar_apoyo_profesoral", p, commandType: CommandType.StoredProcedure);
    }
}
