using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

// SQL vive en db/version_2.sql. Cada método llama un procedimiento explícito.
public class RepositorioReconocimientoSqlServer : IRepositorioReconocimiento
{
    static RepositorioReconocimientoSqlServer() => FechasSql.Configurar();
    private readonly string cadena;
    public RepositorioReconocimientoSqlServer(IConfiguration configuracion) => cadena = configuracion.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("Falta la conexión SqlServer.");

    internal static DynamicParameters Parametros(Reconocimiento entidad)
    {
        var p = new DynamicParameters();
        p.Add("Tipo", entidad.Tipo);
        p.Add("Fecha", entidad.Fecha.ToDateTime(TimeOnly.MinValue));
        p.Add("Institucion", entidad.Institucion);
        p.Add("Nombre", entidad.Nombre);
        p.Add("Ambito", entidad.Ambito);
        p.Add("Docente", entidad.Docente);
        return p;
    }

    public async Task<IEnumerable<Reconocimiento>> ObtenerTodos(int limite)
    {
        await using var conexion = new SqlConnection(cadena);
        return await conexion.QueryAsync<Reconocimiento>("dbo.sp_listar_reconocimiento", new { Limite = limite }, commandType: CommandType.StoredProcedure);
    }

    public async Task<Reconocimiento?> ObtenerPorId(int id)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Id", id);
        return await conexion.QuerySingleOrDefaultAsync<Reconocimiento>("dbo.sp_consultar_reconocimiento", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> Crear(Reconocimiento reconocimiento)
    {
        await using var conexion = new SqlConnection(cadena);
        var guardada = await conexion.QuerySingleAsync<Reconocimiento>("dbo.sp_crear_reconocimiento", Parametros(reconocimiento), commandType: CommandType.StoredProcedure);
        reconocimiento.Id = guardada.Id;
        reconocimiento.Tipo = guardada.Tipo;
        reconocimiento.Fecha = guardada.Fecha;
        reconocimiento.Institucion = guardada.Institucion;
        reconocimiento.Nombre = guardada.Nombre;
        reconocimiento.Ambito = guardada.Ambito;
        reconocimiento.Docente = guardada.Docente;
        return guardada.Id;
    }

    public async Task<int> Reemplazar(int id, Reconocimiento reconocimiento)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = Parametros(reconocimiento);
        p.Add("OriginalId", id);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_reconocimiento", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> ActualizarParcial(int id, ReconocimientoCampos campos)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Tipo", campos.Tipo);
        p.Add("Fecha", campos.Fecha?.ToDateTime(TimeOnly.MinValue));
        p.Add("Institucion", campos.Institucion);
        p.Add("Nombre", campos.Nombre);
        p.Add("Ambito", campos.Ambito);
        p.Add("Docente", campos.Docente);
        p.Add("Parcial", true);
        p.Add("OriginalId", id);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_reconocimiento", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> EliminarLogico(int id)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Id", id);
        return await conexion.QuerySingleAsync<int>("dbo.sp_eliminar_reconocimiento", p, commandType: CommandType.StoredProcedure);
    }
}
