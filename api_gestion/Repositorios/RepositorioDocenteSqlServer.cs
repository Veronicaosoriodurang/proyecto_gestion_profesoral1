using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

/// <summary>
/// Capa 3 contra SQL Server para docente: SQL a mano y siempre parametrizado.
/// Todas las consultas filtran por activo = 1.
/// </summary>
public class RepositorioDocenteSqlServer : IRepositorioDocente
{
    private sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
    {
        public override void SetValue(IDbDataParameter parameter, DateOnly value)
        {
            parameter.Value = value.ToDateTime(TimeOnly.MinValue);
            parameter.DbType = DbType.Date;
        }

        public override DateOnly Parse(object value)
        {
            return value switch
            {
                DateTime fecha => DateOnly.FromDateTime(fecha),
                DateOnly fecha => fecha,
                _ => throw new DataException($"No se puede convertir {value.GetType()} a DateOnly.")
            };
        }
    }

    static RepositorioDocenteSqlServer()
    {
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
    }

    private readonly string _cadenaConexion;

    public RepositorioDocenteSqlServer(IConfiguration configuracion)
    {
        _cadenaConexion = configuracion.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'SqlServer'.");
    }

    private IDbConnection CrearConexion() => new SqlConnection(_cadenaConexion);

    private const string COLUMNAS = @"cedula AS Cedula, nombres AS Nombres, apellidos AS Apellidos,
            genero AS Genero, cargo AS Cargo, fecha_nacimiento AS FechaNacimiento,
            correo AS Correo, telefono AS Telefono, url_cvlac AS UrlCvlac,
            fecha_actualizacion AS FechaActualizacion, escalafon AS Escalafon,
            perfil AS Perfil, cat_minciencia AS CatMinciencia,
            conv_minciencia AS ConvMinciencia, nacionalidaad AS Nacionalidaad,
            linea_investigacion_principal AS LineaInvestigacionPrincipal";

    public async Task<IEnumerable<Docente>> ObtenerTodos(int limite)
    {
        using var conexion = CrearConexion();
        var sql = $@"
            SELECT TOP (@Limite) {COLUMNAS}
            FROM docente
            WHERE activo = 1
            ORDER BY cedula ASC";

        return await conexion.QueryAsync<Docente>(sql, new { Limite = limite });
    }

    public async Task<Docente?> ObtenerPorId(int cedula)
    {
        using var conexion = CrearConexion();
        var sql = $@"
            SELECT {COLUMNAS}
            FROM docente
            WHERE cedula = @Cedula AND activo = 1";

        return await conexion.QueryFirstOrDefaultAsync<Docente>(sql, new { Cedula = cedula });
    }

    public async Task Crear(Docente docente)
    {
        using var conexion = CrearConexion();
        const string sql = @"
            INSERT INTO docente (cedula, nombres, apellidos, genero, cargo, fecha_nacimiento,
                                 correo, telefono, url_cvlac, fecha_actualizacion, escalafon,
                                 perfil, cat_minciencia, conv_minciencia, nacionalidaad,
                                 linea_investigacion_principal, activo)
            VALUES (@Cedula, @Nombres, @Apellidos, @Genero, @Cargo, @FechaNacimiento,
                    @Correo, @Telefono, @UrlCvlac, @FechaActualizacion, @Escalafon,
                    @Perfil, @CatMinciencia, @ConvMinciencia, @Nacionalidaad,
                    @LineaInvestigacionPrincipal, 1)";

        await conexion.ExecuteAsync(sql, new
        {
            docente.Cedula,
            docente.Nombres,
            docente.Apellidos,
            docente.Genero,
            docente.Cargo,
            FechaNacimiento = docente.FechaNacimiento.ToDateTime(TimeOnly.MinValue),
            docente.Correo,
            docente.Telefono,
            docente.UrlCvlac,
            FechaActualizacion = docente.FechaActualizacion.ToDateTime(TimeOnly.MinValue),
            docente.Escalafon,
            docente.Perfil,
            docente.CatMinciencia,
            docente.ConvMinciencia,
            docente.Nacionalidaad,
            docente.LineaInvestigacionPrincipal
        });
    }

    public async Task<int> Reemplazar(Docente docente)
    {
        using var conexion = CrearConexion();
        const string sql = @"
            UPDATE docente
            SET nombres = @Nombres, apellidos = @Apellidos, genero = @Genero,
                cargo = @Cargo, fecha_nacimiento = @FechaNacimiento, correo = @Correo,
                telefono = @Telefono, url_cvlac = @UrlCvlac,
                fecha_actualizacion = @FechaActualizacion, escalafon = @Escalafon,
                perfil = @Perfil, cat_minciencia = @CatMinciencia,
                conv_minciencia = @ConvMinciencia, nacionalidaad = @Nacionalidaad,
                linea_investigacion_principal = @LineaInvestigacionPrincipal
            WHERE cedula = @Cedula AND activo = 1";

        return await conexion.ExecuteAsync(sql, new
        {
            docente.Cedula,
            docente.Nombres,
            docente.Apellidos,
            docente.Genero,
            docente.Cargo,
            FechaNacimiento = docente.FechaNacimiento.ToDateTime(TimeOnly.MinValue),
            docente.Correo,
            docente.Telefono,
            docente.UrlCvlac,
            FechaActualizacion = docente.FechaActualizacion.ToDateTime(TimeOnly.MinValue),
            docente.Escalafon,
            docente.Perfil,
            docente.CatMinciencia,
            docente.ConvMinciencia,
            docente.Nacionalidaad,
            docente.LineaInvestigacionPrincipal
        });
    }

    public async Task<int> ActualizarParcial(int cedula, DocenteCampos campos)
    {
        using var conexion = CrearConexion();

        var asignaciones = new List<string>();
        var parametros = new DynamicParameters();
        parametros.Add("Cedula", cedula);

        void Agregar(string columna, string parametro, object? valor)
        {
            if (valor == null) return;
            asignaciones.Add($"{columna} = @{parametro}");
            parametros.Add(parametro, valor);
        }

        Agregar("nombres", "Nombres", campos.Nombres);
        Agregar("apellidos", "Apellidos", campos.Apellidos);
        Agregar("genero", "Genero", campos.Genero);
        Agregar("cargo", "Cargo", campos.Cargo);
        if (campos.FechaNacimiento.HasValue)
            Agregar("fecha_nacimiento", "FechaNacimiento",
                campos.FechaNacimiento.Value.ToDateTime(TimeOnly.MinValue));
        Agregar("correo", "Correo", campos.Correo);
        Agregar("telefono", "Telefono", campos.Telefono);
        Agregar("url_cvlac", "UrlCvlac", campos.UrlCvlac);
        if (campos.FechaActualizacion.HasValue)
            Agregar("fecha_actualizacion", "FechaActualizacion",
                campos.FechaActualizacion.Value.ToDateTime(TimeOnly.MinValue));
        Agregar("escalafon", "Escalafon", campos.Escalafon);
        Agregar("perfil", "Perfil", campos.Perfil);
        Agregar("cat_minciencia", "CatMinciencia", campos.CatMinciencia);
        Agregar("conv_minciencia", "ConvMinciencia", campos.ConvMinciencia);
        Agregar("nacionalidaad", "Nacionalidaad", campos.Nacionalidaad);
        Agregar("linea_investigacion_principal", "LineaInvestigacionPrincipal",
            campos.LineaInvestigacionPrincipal);

        if (asignaciones.Count == 0) return 0;

        var sql = $"UPDATE docente SET {string.Join(", ", asignaciones)} WHERE cedula = @Cedula AND activo = 1";

        return await conexion.ExecuteAsync(sql, parametros);
    }

    public async Task<int> EliminarLogico(int cedula)
    {
        using var conexion = CrearConexion();
        const string sql = @"
            UPDATE docente
            SET activo = 0
            WHERE cedula = @Cedula AND activo = 1";

        return await conexion.ExecuteAsync(sql, new { Cedula = cedula });
    }
}

