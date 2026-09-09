using FrontGestion.Components;
using FrontGestion.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Blazor Server: el componente se renderiza en el servidor y el navegador
// recibe el HTML ya armado, manteniendo una conexiÃ³n para los eventos.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ============================================================
// DE DÃ“NDE SALEN LOS DATOS
//
// De la API, por HTTP, y de ningÃºn otro sitio. La direcciÃ³n viene de la
// configuraciÃ³n: fuera de Docker vale lo de appsettings.json; dentro, el
// compose la sobreescribe con el NOMBRE del servicio â€”`api-gestion`â€”,
// porque `localhost` dentro de un contenedor es el contenedor mismo.
// ============================================================
var urlApi = builder.Configuration["UrlApi"] ?? "http://localhost:8074";

builder.Services.AddHttpClient<ServicioPrograma>(cliente =>
{
    cliente.BaseAddress = new Uri(urlApi);
    cliente.Timeout = TimeSpan.FromSeconds(10);
});


builder.Services.AddHttpClient<ServicioRed>(cliente =>
{
    cliente.BaseAddress = new Uri(urlApi);
    cliente.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient<ServicioAreaConocimiento>(cliente =>
{
    cliente.BaseAddress = new Uri(urlApi);
    cliente.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient<ServicioTerminoClave>(cliente =>
{
    cliente.BaseAddress = new Uri(urlApi);
    cliente.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient<ServicioLineaInvestigacion>(cliente =>
{
    cliente.BaseAddress = new Uri(urlApi);
    cliente.Timeout = TimeSpan.FromSeconds(10);
});
// ============================================================
// UN SERVICIO POR RECURSO (ArtÃ­culo 10.1)
//
// Hoy hay uno porque la v1 construye una tabla. Cuando haya mÃ¡s recursos
// habrÃ¡ una lÃ­nea por cada uno â€” no una que sirva para cualquier tabla.
// ============================================================

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();




