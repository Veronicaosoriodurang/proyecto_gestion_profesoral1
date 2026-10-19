using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

// SQL vive en db/version_2.sql. Cada método llama un procedimiento explícito.
public class RepositorioProgramaSqlServer : IRepositorioPrograma
{
    static RepositorioProgramaSqlServer() => FechasSql.Configurar();
    private readonly string cadena;
    public RepositorioProgramaSqlServer(IConfiguration configuracion) => cadena = configuracion.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("Falta la conexión SqlServer.");

    internal static DynamicParameters Parametros(Programa entidad)
    {
        var p = new DynamicParameters();
        p.Add("Id", entidad.Id);
        p.Add("Nombre", entidad.Nombre);
        p.Add("Tipo", entidad.Tipo);
        p.Add("Nivel", entidad.Nivel);
        p.Add("FechaCreacion", entidad.FechaCreacion);
        p.Add("FechaCierre", entidad.FechaCierre);
        p.Add("NumeroCohortes", entidad.NumeroCohortes);
        p.Add("CantGraduados", entidad.CantGraduados);
        p.Add("FechaActualizacion", entidad.FechaActualizacion);
        p.Add("Ciudad", entidad.Ciudad);
        p.Add("Facultad", entidad.Facultad);
        return p;
    }

    public async Task<IEnumerable<Programa>> ObtenerTodos(int limite)
    {
        await using var conexion = new SqlConnection(cadena);
        return await conexion.QueryAsync<Programa>("dbo.sp_listar_programa", new { Limite = limite }, commandType: CommandType.StoredProcedure);
    }

    public async Task<Programa?> ObtenerPorId(int id)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Id", id);
        return await conexion.QuerySingleOrDefaultAsync<Programa>("dbo.sp_consultar_programa", p, commandType: CommandType.StoredProcedure);
    }

    public async Task Crear(Programa programa)
    {
        await using var conexion = new SqlConnection(cadena);
        var guardada = await conexion.QuerySingleAsync<Programa>("dbo.sp_crear_programa", Parametros(programa), commandType: CommandType.StoredProcedure);
        programa.Id = guardada.Id;
        programa.Nombre = guardada.Nombre;
        programa.Tipo = guardada.Tipo;
        programa.Nivel = guardada.Nivel;
        programa.FechaCreacion = guardada.FechaCreacion;
        programa.FechaCierre = guardada.FechaCierre;
        programa.NumeroCohortes = guardada.NumeroCohortes;
        programa.CantGraduados = guardada.CantGraduados;
        programa.FechaActualizacion = guardada.FechaActualizacion;
        programa.Ciudad = guardada.Ciudad;
        programa.Facultad = guardada.Facultad;
    }

    public async Task<int> Reemplazar(Programa programa)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = Parametros(programa);
        p.Add("OriginalId", programa.Id);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_programa", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> ActualizarParcial(int id, ProgramaCampos campos)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Nombre", campos.Nombre);
        p.Add("Tipo", campos.Tipo);
        p.Add("Nivel", campos.Nivel);
        p.Add("FechaCreacion", campos.FechaCreacion);
        p.Add("FechaCierre", campos.FechaCierre);
        p.Add("NumeroCohortes", campos.NumeroCohortes);
        p.Add("CantGraduados", campos.CantGraduados);
        p.Add("FechaActualizacion", campos.FechaActualizacion);
        p.Add("Ciudad", campos.Ciudad);
        p.Add("Facultad", campos.Facultad);
        p.Add("Id", id);
        p.Add("Parcial", true);
        p.Add("OriginalId", id);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_programa", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> EliminarLogico(int id)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Id", id);
        return await conexion.QuerySingleAsync<int>("dbo.sp_eliminar_programa", p, commandType: CommandType.StoredProcedure);
    }
}
