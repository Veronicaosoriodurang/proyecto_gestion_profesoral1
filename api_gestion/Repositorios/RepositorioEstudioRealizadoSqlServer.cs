using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

// SQL vive en db/version_2.sql. Cada método llama un procedimiento explícito.
public class RepositorioEstudioRealizadoSqlServer : IRepositorioEstudioRealizado
{
    static RepositorioEstudioRealizadoSqlServer() => FechasSql.Configurar();
    private readonly string cadena;
    public RepositorioEstudioRealizadoSqlServer(IConfiguration configuracion) => cadena = configuracion.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("Falta la conexión SqlServer.");

    internal static DynamicParameters Parametros(EstudioRealizado entidad)
    {
        var p = new DynamicParameters();
        p.Add("Id", entidad.Id);
        p.Add("Titulo", entidad.Titulo);
        p.Add("Universidad", entidad.Universidad);
        p.Add("Fecha", entidad.Fecha.ToDateTime(TimeOnly.MinValue));
        p.Add("Tipo", entidad.Tipo);
        p.Add("Ciudad", entidad.Ciudad);
        p.Add("Docente", entidad.Docente);
        p.Add("InsAcreditada", entidad.InsAcreditada);
        p.Add("Metodologia", entidad.Metodologia);
        p.Add("PerfilEgresado", entidad.PerfilEgresado);
        p.Add("Pais", entidad.Pais);
        return p;
    }

    public async Task<IEnumerable<EstudioRealizado>> ObtenerTodos(int limite)
    {
        await using var conexion = new SqlConnection(cadena);
        return await conexion.QueryAsync<EstudioRealizado>("dbo.sp_listar_estudios_realizados", new { Limite = limite }, commandType: CommandType.StoredProcedure);
    }

    public async Task<EstudioRealizado?> ObtenerPorId(int id)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Id", id);
        return await conexion.QuerySingleOrDefaultAsync<EstudioRealizado>("dbo.sp_consultar_estudios_realizados", p, commandType: CommandType.StoredProcedure);
    }

    public async Task Crear(EstudioRealizado estudio)
    {
        await using var conexion = new SqlConnection(cadena);
        var guardada = await conexion.QuerySingleAsync<EstudioRealizado>("dbo.sp_crear_estudios_realizados", Parametros(estudio), commandType: CommandType.StoredProcedure);
        estudio.Id = guardada.Id;
        estudio.Titulo = guardada.Titulo;
        estudio.Universidad = guardada.Universidad;
        estudio.Fecha = guardada.Fecha;
        estudio.Tipo = guardada.Tipo;
        estudio.Ciudad = guardada.Ciudad;
        estudio.Docente = guardada.Docente;
        estudio.InsAcreditada = guardada.InsAcreditada;
        estudio.Metodologia = guardada.Metodologia;
        estudio.PerfilEgresado = guardada.PerfilEgresado;
        estudio.Pais = guardada.Pais;
    }

    public async Task<int> Reemplazar(EstudioRealizado estudio)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = Parametros(estudio);
        p.Add("OriginalId", estudio.Id);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_estudios_realizados", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> ActualizarParcial(int id, EstudioRealizadoCampos campos)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Titulo", campos.Titulo);
        p.Add("Universidad", campos.Universidad);
        p.Add("Fecha", campos.Fecha?.ToDateTime(TimeOnly.MinValue));
        p.Add("Tipo", campos.Tipo);
        p.Add("Ciudad", campos.Ciudad);
        p.Add("Docente", campos.Docente);
        p.Add("InsAcreditada", campos.InsAcreditada);
        p.Add("Metodologia", campos.Metodologia);
        p.Add("PerfilEgresado", campos.PerfilEgresado);
        p.Add("Pais", campos.Pais);
        p.Add("Id", id);
        p.Add("Parcial", true);
        p.Add("OriginalId", id);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_estudios_realizados", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> EliminarLogico(int id)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Id", id);
        return await conexion.QuerySingleAsync<int>("dbo.sp_eliminar_estudios_realizados", p, commandType: CommandType.StoredProcedure);
    }
}
