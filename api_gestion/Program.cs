using ApiGestion.Repositorios;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// La API escucha en el 8074 tambiÃ©n DENTRO del contenedor, para que el Dockerfile,
// el docker-compose y los contratos digan todos el mismo nÃºmero (3_plan.md Â§5.2).
builder.WebHost.UseUrls("http://0.0.0.0:8074");

// ============================================================
// EL ENSAMBLADOR (ArtÃ­culo 3)
// Estas dos lÃ­neas son el ÃšNICO lugar donde una clase concreta aparece junto a su
// interfaz. Todo lo demÃ¡s recibe interfaces por constructor.
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
builder.Services.AddControllers();

// ============================================================
// EL 422 DEL CONTRATO (3_plan.md Â§4.9)
// Con [ApiController], un cuerpo invÃ¡lido corta la peticiÃ³n ANTES de entrar al
// mÃ©todo y responde 400 con ProblemDetails. El contrato exige 422 con el sobre
// {estado, mensaje, errores[]}: hay que reemplazar la fÃ¡brica de respuestas.
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
            mensaje = "Datos invÃ¡lidos.",
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

// RF7 â€” DiagnÃ³stico: dice quiÃ©n es y quÃ© versiÃ³n, sin tocar la base de datos.
app.MapGet("/", () => Results.Ok(new
{
    mensaje = "API GestiÃ³n Profesoral â€” mÃ³dulo de programas",
    version = "v1",
    contratos = "/swagger"
}));

app.MapControllers();

app.Run();



