using ApiGestion.Repositorios;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// La API escucha en el 8074 también DENTRO del contenedor, para que el Dockerfile,
// el docker-compose y los contratos digan todos el mismo número (3_plan.md §5.2).
builder.WebHost.UseUrls("http://0.0.0.0:8074");

// ============================================================
// EL ENSAMBLADOR (Artículo 3)
// Estas dos líneas son el ÚNICO lugar donde una clase concreta aparece junto a su
// interfaz. Todo lo demás recibe interfaces por constructor.
// ============================================================
builder.Services.AddScoped<IRepositorioPrograma, RepositorioProgramaSqlServer>();
builder.Services.AddScoped<IServicioPrograma, ServicioPrograma>();
builder.Services.AddScoped<IRepositorioRed, RepositorioRedSqlServer>();
builder.Services.AddScoped<IServicioRed, ServicioRed>();

builder.Services.AddScoped<IRepositorioAreaConocimiento, RepositorioAreaConocimientoSqlServer>();
builder.Services.AddScoped<IServicioAreaConocimiento, ServicioAreaConocimiento>();

builder.Services.AddScoped<IRepositorioTerminoClave, RepositorioTerminoClaveSqlServer>();
builder.Services.AddScoped<IServicioTerminoClave, ServicioTerminoClave>();

builder.Services.AddScoped<IRepositorioLineaInvestigacion, RepositorioLineaInvestigacionSqlServer>();
builder.Services.AddScoped<IServicioLineaInvestigacion, ServicioLineaInvestigacion>();

builder.Services.AddScoped<IRepositorioDocente, RepositorioDocenteSqlServer>();
builder.Services.AddScoped<IServicioDocente, ServicioDocente>();
builder.Services.AddScoped<IRepositorioEstudioRealizado, RepositorioEstudioRealizadoSqlServer>();
builder.Services.AddScoped<IServicioEstudioRealizado, ServicioEstudioRealizado>();
builder.Services.AddScoped<IRepositorioDocenteDepartamento, RepositorioDocenteDepartamentoSqlServer>();
builder.Services.AddScoped<IServicioDocenteDepartamento, ServicioDocenteDepartamento>();
builder.Services.AddScoped<IRepositorioInteresFuturo, RepositorioInteresFuturoSqlServer>();
builder.Services.AddScoped<IServicioInteresFuturo, ServicioInteresFuturo>();
builder.Services.AddScoped<IRepositorioEvaluacionDocente, RepositorioEvaluacionDocenteSqlServer>();
builder.Services.AddScoped<IServicioEvaluacionDocente, ServicioEvaluacionDocente>();
builder.Services.AddScoped<IRepositorioReconocimiento, RepositorioReconocimientoSqlServer>();
builder.Services.AddScoped<IServicioReconocimiento, ServicioReconocimiento>();
builder.Services.AddScoped<IRepositorioExperiecia, RepositorioExperieciaSqlServer>();
builder.Services.AddScoped<IServicioExperiecia, ServicioExperiecia>();
builder.Services.AddScoped<IRepositorioRedDocente, RepositorioRedDocenteSqlServer>();
builder.Services.AddScoped<IServicioRedDocente, ServicioRedDocente>();
builder.Services.AddScoped<IRepositorioEstudioAc, RepositorioEstudioAcSqlServer>();
builder.Services.AddScoped<IServicioEstudioAc, ServicioEstudioAc>();
builder.Services.AddScoped<IRepositorioApoyoProfesoral, RepositorioApoyoProfesoralSqlServer>();
builder.Services.AddScoped<IServicioApoyoProfesoral, ServicioApoyoProfesoral>();

builder.Services.AddControllers();

// ============================================================
// EL 422 DEL CONTRATO (3_plan.md §4.9)
// Con [ApiController], un cuerpo inválido corta la petición ANTES de entrar al
// método y responde 400 con ProblemDetails. El contrato exige 422 con el sobre
// {estado, mensaje, errores[]}: hay que reemplazar la fábrica de respuestas.
// ============================================================
builder.Services.Configure<ApiBehaviorOptions>(opciones =>
{
    opciones.InvalidModelStateResponseFactory = contexto =>
    {
        var errores = contexto.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .SelectMany(e => e.Value!.Errors.Select(x => x.ErrorMessage))
            .ToList();

        return new ObjectResult(new
        {
            estado = 422,
            mensaje = "Datos inválidos.",
            errores
        })
        { StatusCode = 422 };
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// RF7 — Diagnóstico: dice quién es y qué versión, sin tocar la base de datos.
app.MapGet("/", () => Results.Ok(new
{
    mensaje = "API Gestión Profesoral — módulo de programas",
    version = "v1",
    contratos = "/swagger"
}));

app.MapControllers();

app.Run();



