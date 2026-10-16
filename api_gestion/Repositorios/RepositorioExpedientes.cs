using System.Data;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Excepciones;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public interface IRepositorioExpedientes
{
    Task<Docente> GuardarDocente(int? cedula, ExpedienteDocenteCrear peticion);
    Task<EstudioRealizado> GuardarEstudio(int? id, ExpedienteEstudioCrear peticion);
}

public class RepositorioExpedientes(IConfiguration configuracion) : IRepositorioExpedientes
{
    // Una conexión y una transacción para el maestro y todos los renglones.
    public async Task<Docente> GuardarDocente(int? clave, ExpedienteDocenteCrear peticion)
    {
        FechasSql.Configurar();
        await using var conexion = new SqlConnection(configuracion.GetConnectionString("SqlServer"));
        await conexion.OpenAsync();
        await using var tx = (SqlTransaction)await conexion.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            var entidad = Convertir(peticion.Docente);
            if (clave.HasValue)
            {
                entidad.Cedula = clave.Value;
                var existe = await conexion.QuerySingleOrDefaultAsync<Docente>("dbo.sp_consultar_docente", new { Cedula = clave.Value }, tx, commandType: CommandType.StoredProcedure);
                if (existe == null) throw new NoEncontradoExcepcion("El registro maestro no existe o está retirado.");
            }
            else
                entidad = await conexion.QuerySingleAsync<Docente>("dbo.sp_crear_docente", RepositorioDocenteSqlServer.Parametros(entidad), tx, commandType: CommandType.StoredProcedure);
            foreach (var fila in peticion.Detalles.Estudios)
            {
                var detalle = Convertir(fila);
                detalle.Docente = entidad.Cedula;
                await conexion.QuerySingleAsync<EstudioRealizado>("dbo.sp_crear_estudios_realizados", RepositorioEstudioRealizadoSqlServer.Parametros(detalle), tx, commandType: CommandType.StoredProcedure);
            }
            foreach (var fila in peticion.Detalles.Evaluaciones)
            {
                var detalle = Convertir(fila);
                detalle.Docente = entidad.Cedula;
                await conexion.QuerySingleAsync<EvaluacionDocente>("dbo.sp_crear_evaluacion_docente", RepositorioEvaluacionDocenteSqlServer.Parametros(detalle), tx, commandType: CommandType.StoredProcedure);
            }
            foreach (var fila in peticion.Detalles.Experiencias)
            {
                var detalle = Convertir(fila);
                detalle.Docente = entidad.Cedula;
                await conexion.QuerySingleAsync<Experiecia>("dbo.sp_crear_experiecia", RepositorioExperieciaSqlServer.Parametros(detalle), tx, commandType: CommandType.StoredProcedure);
            }
            foreach (var fila in peticion.Detalles.Reconocimientos)
            {
                var detalle = Convertir(fila);
                detalle.Docente = entidad.Cedula;
                await conexion.QuerySingleAsync<Reconocimiento>("dbo.sp_crear_reconocimiento", RepositorioReconocimientoSqlServer.Parametros(detalle), tx, commandType: CommandType.StoredProcedure);
            }
            foreach (var fila in peticion.Detalles.Departamentos)
            {
                var detalle = Convertir(fila);
                detalle.Docente = entidad.Cedula;
                await conexion.QuerySingleAsync<DocenteDepartamento>("dbo.sp_crear_docente_departamento", RepositorioDocenteDepartamentoSqlServer.Parametros(detalle), tx, commandType: CommandType.StoredProcedure);
            }
            foreach (var fila in peticion.Detalles.Intereses)
            {
                var detalle = Convertir(fila);
                detalle.Docente = entidad.Cedula;
                await conexion.QuerySingleAsync<InteresFuturo>("dbo.sp_crear_intereses_futuros", RepositorioInteresFuturoSqlServer.Parametros(detalle), tx, commandType: CommandType.StoredProcedure);
            }
            foreach (var fila in peticion.Detalles.Redes)
            {
                var detalle = Convertir(fila);
                detalle.Docente = entidad.Cedula;
                await conexion.QuerySingleAsync<RedDocente>("dbo.sp_crear_red_docente", RepositorioRedDocenteSqlServer.Parametros(detalle), tx, commandType: CommandType.StoredProcedure);
            }
            var guardada = await conexion.QuerySingleAsync<Docente>("dbo.sp_consultar_docente", new { Cedula = entidad.Cedula }, tx, commandType: CommandType.StoredProcedure);
            await tx.CommitAsync();
            return guardada;
        }
        catch { if (tx.Connection != null) await tx.RollbackAsync(); throw; }
    }
    // Una conexión y una transacción para el maestro y todos los renglones.
    public async Task<EstudioRealizado> GuardarEstudio(int? clave, ExpedienteEstudioCrear peticion)
    {
        FechasSql.Configurar();
        await using var conexion = new SqlConnection(configuracion.GetConnectionString("SqlServer"));
        await conexion.OpenAsync();
        await using var tx = (SqlTransaction)await conexion.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            var entidad = Convertir(peticion.Estudio);
            if (clave.HasValue)
            {
                entidad.Id = clave.Value;
                var existe = await conexion.QuerySingleOrDefaultAsync<EstudioRealizado>("dbo.sp_consultar_estudios_realizados", new { Id = clave.Value }, tx, commandType: CommandType.StoredProcedure);
                if (existe == null) throw new NoEncontradoExcepcion("El registro maestro no existe o está retirado.");
            }
            else
                entidad = await conexion.QuerySingleAsync<EstudioRealizado>("dbo.sp_crear_estudios_realizados", RepositorioEstudioRealizadoSqlServer.Parametros(entidad), tx, commandType: CommandType.StoredProcedure);
            foreach (var fila in peticion.Detalles.Areas)
            {
                var detalle = Convertir(fila);
                detalle.Estudio = entidad.Id;
                await conexion.QuerySingleAsync<EstudioAc>("dbo.sp_crear_estudio_ac", RepositorioEstudioAcSqlServer.Parametros(detalle), tx, commandType: CommandType.StoredProcedure);
            }
            foreach (var fila in peticion.Detalles.Apoyos)
            {
                var detalle = Convertir(fila);
                detalle.Estudios = entidad.Id;
                await conexion.QuerySingleAsync<ApoyoProfesoral>("dbo.sp_crear_apoyo_profesoral", RepositorioApoyoProfesoralSqlServer.Parametros(detalle), tx, commandType: CommandType.StoredProcedure);
            }
            foreach (var fila in peticion.Detalles.Becas)
            {
                var detalle = Convertir(fila);
                detalle.Estudios = entidad.Id;
                await conexion.QuerySingleAsync<Beca>("dbo.sp_crear_beca", RepositorioBecaSqlServer.Parametros(detalle), tx, commandType: CommandType.StoredProcedure);
            }
            var guardada = await conexion.QuerySingleAsync<EstudioRealizado>("dbo.sp_consultar_estudios_realizados", new { Id = entidad.Id }, tx, commandType: CommandType.StoredProcedure);
            await tx.CommitAsync();
            return guardada;
        }
        catch { if (tx.Connection != null) await tx.RollbackAsync(); throw; }
    }
    private static Docente Convertir(DocenteCrear p) => new()
    {
        Cedula = p.Cedula!.Value,
        Nombres = p.Nombres,
        Apellidos = p.Apellidos,
        Genero = p.Genero,
        Cargo = p.Cargo,
        FechaNacimiento = p.FechaNacimiento!.Value,
        Correo = p.Correo,
        Telefono = p.Telefono,
        UrlCvlac = p.UrlCvlac,
        FechaActualizacion = p.FechaActualizacion!.Value,
        Escalafon = p.Escalafon,
        Perfil = p.Perfil,
        CatMinciencia = p.CatMinciencia,
        ConvMinciencia = p.ConvMinciencia,
        Nacionalidaad = p.Nacionalidaad,
        LineaInvestigacionPrincipal = p.LineaInvestigacionPrincipal,
    };
    private static EstudioRealizado Convertir(EstudioRealizadoCrear p) => new()
    {
        Id = p.Id!.Value,
        Titulo = p.Titulo,
        Universidad = p.Universidad,
        Fecha = p.Fecha!.Value,
        Tipo = p.Tipo,
        Ciudad = p.Ciudad,
        Docente = p.Docente!.Value,
        InsAcreditada = p.InsAcreditada!.Value,
        Metodologia = p.Metodologia,
        PerfilEgresado = p.PerfilEgresado,
        Pais = p.Pais,
    };
    private static DocenteDepartamento Convertir(DocenteDepartamentoCrear p) => new()
    {
        Docente = p.Docente!.Value,
        Departamento = p.Departamento!.Value,
        Dedicacion = p.Dedicacion,
        Modalidad = p.Modalidad,
        FechaIngreso = p.FechaIngreso!.Value,
        FechaSalida = p.FechaSalida,
    };
    private static InteresFuturo Convertir(InteresFuturoCrear p) => new()
    {
        Docente = p.Docente!.Value,
        TerminoClave = p.TerminoClave,
    };
    private static EvaluacionDocente Convertir(EvaluacionDocenteCrear p) => new()
    {
        Calificacion = p.Calificacion!.Value,
        Semestre = p.Semestre,
        Docente = p.Docente!.Value,
    };
    private static Reconocimiento Convertir(ReconocimientoCrear p) => new()
    {
        Tipo = p.Tipo,
        Fecha = p.Fecha!.Value,
        Institucion = p.Institucion,
        Nombre = p.Nombre,
        Ambito = p.Ambito,
        Docente = p.Docente!.Value,
    };
    private static Experiecia Convertir(ExperieciaCrear p) => new()
    {
        NombreCargo = p.NombreCargo,
        Institucion = p.Institucion,
        Tipo = p.Tipo,
        FechaInicio = p.FechaInicio!.Value,
        FechaFin = p.FechaFin,
        Docente = p.Docente!.Value,
    };
    private static RedDocente Convertir(RedDocenteCrear p) => new()
    {
        Red = p.Red!.Value,
        Docente = p.Docente!.Value,
        FechaInicio = p.FechaInicio!.Value,
        FechaFin = p.FechaFin,
        ActDestacadas = p.ActDestacadas,
    };
    private static EstudioAc Convertir(EstudioAcCrear p) => new()
    {
        Estudio = p.Estudio!.Value,
        AreaConocimiento = p.AreaConocimiento,
    };
    private static ApoyoProfesoral Convertir(ApoyoProfesoralCrear p) => new()
    {
        Estudios = p.Estudios!.Value,
        ConApoyo = p.ConApoyo!.Value,
        Institucion = p.Institucion,
        Tipo = p.Tipo,
    };
    private static Beca Convertir(BecaCrear p) => new()
    {
        Estudios = p.Estudios!.Value,
        Tipo = p.Tipo,
        Institucion = p.Institucion,
        FechaInicio = p.FechaInicio!.Value,
        FechaFin = p.FechaFin,
    };
}
