using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

// SQL vive en db/version_2.sql. Cada método llama un procedimiento explícito.
public class RepositorioDocenteSqlServer : IRepositorioDocente
{
    static RepositorioDocenteSqlServer() => FechasSql.Configurar();
    private readonly string cadena;
    public RepositorioDocenteSqlServer(IConfiguration configuracion) => cadena = configuracion.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("Falta la conexión SqlServer.");

    internal static DynamicParameters Parametros(Docente entidad)
    {
        var p = new DynamicParameters();
        p.Add("Cedula", entidad.Cedula);
        p.Add("Nombres", entidad.Nombres);
        p.Add("Apellidos", entidad.Apellidos);
        p.Add("Genero", entidad.Genero);
        p.Add("Cargo", entidad.Cargo);
        p.Add("FechaNacimiento", entidad.FechaNacimiento.ToDateTime(TimeOnly.MinValue));
        p.Add("Correo", entidad.Correo);
        p.Add("Telefono", entidad.Telefono);
        p.Add("UrlCvlac", entidad.UrlCvlac);
        p.Add("FechaActualizacion", entidad.FechaActualizacion.ToDateTime(TimeOnly.MinValue));
        p.Add("Escalafon", entidad.Escalafon);
        p.Add("Perfil", entidad.Perfil);
        p.Add("CatMinciencia", entidad.CatMinciencia);
        p.Add("ConvMinciencia", entidad.ConvMinciencia);
        p.Add("Nacionalidaad", entidad.Nacionalidaad);
        p.Add("LineaInvestigacionPrincipal", entidad.LineaInvestigacionPrincipal);
        return p;
    }

    public async Task<IEnumerable<Docente>> ObtenerTodos(int limite)
    {
        await using var conexion = new SqlConnection(cadena);
        return await conexion.QueryAsync<Docente>("dbo.sp_listar_docente", new { Limite = limite }, commandType: CommandType.StoredProcedure);
    }

    public async Task<Docente?> ObtenerPorId(int cedula)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Cedula", cedula);
        return await conexion.QuerySingleOrDefaultAsync<Docente>("dbo.sp_consultar_docente", p, commandType: CommandType.StoredProcedure);
    }

    public async Task Crear(Docente docente)
    {
        await using var conexion = new SqlConnection(cadena);
        var guardada = await conexion.QuerySingleAsync<Docente>("dbo.sp_crear_docente", Parametros(docente), commandType: CommandType.StoredProcedure);
        docente.Cedula = guardada.Cedula;
        docente.Nombres = guardada.Nombres;
        docente.Apellidos = guardada.Apellidos;
        docente.Genero = guardada.Genero;
        docente.Cargo = guardada.Cargo;
        docente.FechaNacimiento = guardada.FechaNacimiento;
        docente.Correo = guardada.Correo;
        docente.Telefono = guardada.Telefono;
        docente.UrlCvlac = guardada.UrlCvlac;
        docente.FechaActualizacion = guardada.FechaActualizacion;
        docente.Escalafon = guardada.Escalafon;
        docente.Perfil = guardada.Perfil;
        docente.CatMinciencia = guardada.CatMinciencia;
        docente.ConvMinciencia = guardada.ConvMinciencia;
        docente.Nacionalidaad = guardada.Nacionalidaad;
        docente.LineaInvestigacionPrincipal = guardada.LineaInvestigacionPrincipal;
        docente.TotalEstudios = guardada.TotalEstudios;
    }

    public async Task<int> Reemplazar(Docente docente)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = Parametros(docente);
        p.Add("OriginalCedula", docente.Cedula);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_docente", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> ActualizarParcial(int cedula, DocenteCampos campos)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Nombres", campos.Nombres);
        p.Add("Apellidos", campos.Apellidos);
        p.Add("Genero", campos.Genero);
        p.Add("Cargo", campos.Cargo);
        p.Add("FechaNacimiento", campos.FechaNacimiento?.ToDateTime(TimeOnly.MinValue));
        p.Add("Correo", campos.Correo);
        p.Add("Telefono", campos.Telefono);
        p.Add("UrlCvlac", campos.UrlCvlac);
        p.Add("FechaActualizacion", campos.FechaActualizacion?.ToDateTime(TimeOnly.MinValue));
        p.Add("Escalafon", campos.Escalafon);
        p.Add("Perfil", campos.Perfil);
        p.Add("CatMinciencia", campos.CatMinciencia);
        p.Add("ConvMinciencia", campos.ConvMinciencia);
        p.Add("Nacionalidaad", campos.Nacionalidaad);
        p.Add("LineaInvestigacionPrincipal", campos.LineaInvestigacionPrincipal);
        p.Add("Cedula", cedula);
        p.Add("Parcial", true);
        p.Add("OriginalCedula", cedula);
        return await conexion.QuerySingleAsync<int>("dbo.sp_actualizar_docente", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> EliminarLogico(int cedula)
    {
        await using var conexion = new SqlConnection(cadena);
        var p = new DynamicParameters();
        p.Add("Cedula", cedula);
        return await conexion.QuerySingleAsync<int>("dbo.sp_eliminar_docente", p, commandType: CommandType.StoredProcedure);
    }
}
