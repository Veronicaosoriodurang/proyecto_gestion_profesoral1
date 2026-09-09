using System.Net.Http.Json;
using System.Text.Json;

namespace FrontGestion.Servicios;

/// <summary>
/// Docente tal como el front lo maneja. Clase propia del front (no compartida
/// con la API): los une solo el JSON.
/// </summary>
public class Docente
{
    public int Cedula { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public DateOnly? FechaNacimiento { get; set; }
    public string Correo { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string UrlCvlac { get; set; } = string.Empty;
    public DateOnly? FechaActualizacion { get; set; }
    public string Escalafon { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
    public string? CatMinciencia { get; set; }
    public string ConvMinciencia { get; set; } = string.Empty;
    public string Nacionalidaad { get; set; } = string.Empty;
    public int? LineaInvestigacionPrincipal { get; set; }
}

/// <summary>
/// Capa de datos del front para el recurso docente. Reutiliza Resultado&lt;T&gt;
/// definido en ServicioPrograma.cs (mismo namespace).
/// </summary>
public class ServicioDocente
{
    private readonly HttpClient _http;

    private static readonly JsonSerializerOptions _opciones = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly List<string> NoDisponible = new()
    {
        "El servicio no está disponible. ¿Está arriba la API?"
    };

    public ServicioDocente(HttpClient http)
    {
        _http = http;
    }

    public async Task<Resultado<List<Docente>>> Listar(int limite = 1000)
    {
        try
        {
            var r = await _http.GetAsync($"/api/docente?limite={limite}");

            if (r.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return Resultado<List<Docente>>.Bien(new());
            }

            if (!r.IsSuccessStatusCode)
            {
                return Resultado<List<Docente>>.Mal(await Mensajes(r));
            }

            var sobre = await r.Content.ReadFromJsonAsync<JsonElement>();
            var datos = sobre.GetProperty("datos")
                .Deserialize<List<Docente>>(_opciones) ?? new();

            return Resultado<List<Docente>>.Bien(datos);
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            return Resultado<List<Docente>>.Mal(NoDisponible);
        }
    }

    public async Task<Resultado<Docente>> Obtener(int cedula)
    {
        try
        {
            var r = await _http.GetAsync($"/api/docente/{cedula}");
            if (!r.IsSuccessStatusCode)
            {
                return Resultado<Docente>.Mal(await Mensajes(r));
            }

            var ficha = await r.Content.ReadFromJsonAsync<Docente>(_opciones);
            return Resultado<Docente>.Bien(ficha!);
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            return Resultado<Docente>.Mal(NoDisponible);
        }
    }

    public async Task<Resultado<bool>> Crear(Docente entidad)
    {
        var cuerpo = new
        {
            cedula = entidad.Cedula,
            nombres = entidad.Nombres,
            apellidos = entidad.Apellidos,
            genero = entidad.Genero,
            cargo = entidad.Cargo,
            fechaNacimiento = entidad.FechaNacimiento,
            correo = entidad.Correo,
            telefono = entidad.Telefono,
            urlCvlac = entidad.UrlCvlac,
            fechaActualizacion = entidad.FechaActualizacion,
            escalafon = entidad.Escalafon,
            perfil = entidad.Perfil,
            catMinciencia = entidad.CatMinciencia,
            convMinciencia = entidad.ConvMinciencia,
            nacionalidaad = entidad.Nacionalidaad,
            lineaInvestigacionPrincipal = entidad.LineaInvestigacionPrincipal
        };
        return await Enviar(HttpMethod.Post, "/api/docente", cuerpo);
    }

    public async Task<Resultado<bool>> Reemplazar(int cedula, Docente entidad)
    {
        var cuerpo = new
        {
            nombres = entidad.Nombres,
            apellidos = entidad.Apellidos,
            genero = entidad.Genero,
            cargo = entidad.Cargo,
            fechaNacimiento = entidad.FechaNacimiento,
            correo = entidad.Correo,
            telefono = entidad.Telefono,
            urlCvlac = entidad.UrlCvlac,
            fechaActualizacion = entidad.FechaActualizacion,
            escalafon = entidad.Escalafon,
            perfil = entidad.Perfil,
            catMinciencia = entidad.CatMinciencia,
            convMinciencia = entidad.ConvMinciencia,
            nacionalidaad = entidad.Nacionalidaad,
            lineaInvestigacionPrincipal = entidad.LineaInvestigacionPrincipal
        };
        return await Enviar(HttpMethod.Put, $"/api/docente/{cedula}", cuerpo);
    }

    public async Task<Resultado<bool>> Actualizar(
        int cedula,
        string? nombres,
        string? apellidos,
        string? genero,
        string? cargo,
        DateOnly? fechaNacimiento,
        string? correo,
        string? telefono,
        string? urlCvlac,
        DateOnly? fechaActualizacion,
        string? escalafon,
        string? perfil,
        string? catMinciencia,
        string? convMinciencia,
        string? nacionalidaad,
        int? lineaInvestigacionPrincipal)
    {
        var cuerpo = new Dictionary<string, object?>();
        if (!string.IsNullOrWhiteSpace(nombres)) cuerpo["nombres"] = nombres;
        if (!string.IsNullOrWhiteSpace(apellidos)) cuerpo["apellidos"] = apellidos;
        if (!string.IsNullOrWhiteSpace(genero)) cuerpo["genero"] = genero;
        if (!string.IsNullOrWhiteSpace(cargo)) cuerpo["cargo"] = cargo;
        if (fechaNacimiento.HasValue) cuerpo["fechaNacimiento"] = fechaNacimiento.Value;
        if (!string.IsNullOrWhiteSpace(correo)) cuerpo["correo"] = correo;
        if (!string.IsNullOrWhiteSpace(telefono)) cuerpo["telefono"] = telefono;
        if (!string.IsNullOrWhiteSpace(urlCvlac)) cuerpo["urlCvlac"] = urlCvlac;
        if (fechaActualizacion.HasValue) cuerpo["fechaActualizacion"] = fechaActualizacion.Value;
        if (!string.IsNullOrWhiteSpace(escalafon)) cuerpo["escalafon"] = escalafon;
        if (!string.IsNullOrWhiteSpace(perfil)) cuerpo["perfil"] = perfil;
        if (!string.IsNullOrWhiteSpace(catMinciencia)) cuerpo["catMinciencia"] = catMinciencia;
        if (!string.IsNullOrWhiteSpace(convMinciencia)) cuerpo["convMinciencia"] = convMinciencia;
        if (!string.IsNullOrWhiteSpace(nacionalidaad)) cuerpo["nacionalidaad"] = nacionalidaad;
        if (lineaInvestigacionPrincipal.HasValue)
            cuerpo["lineaInvestigacionPrincipal"] = lineaInvestigacionPrincipal.Value;

        return await Enviar(HttpMethod.Patch, $"/api/docente/{cedula}", cuerpo);
    }

    public async Task<Resultado<bool>> Eliminar(int cedula)
    {
        return await Enviar(HttpMethod.Delete, $"/api/docente/{cedula}", null);
    }

    private async Task<Resultado<bool>> Enviar(HttpMethod metodo, string ruta, object? cuerpo)
    {
        try
        {
            var peticion = new HttpRequestMessage(metodo, ruta);
            if (cuerpo != null)
            {
                peticion.Content = JsonContent.Create(cuerpo);
            }

            var r = await _http.SendAsync(peticion);
            return r.IsSuccessStatusCode
                ? Resultado<bool>.Bien(true)
                : Resultado<bool>.Mal(await Mensajes(r));
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            return Resultado<bool>.Mal(NoDisponible);
        }
    }

    private static async Task<List<string>> Mensajes(HttpResponseMessage r)
    {
        try
        {
            var sobre = await r.Content.ReadFromJsonAsync<JsonElement>();

            if (sobre.TryGetProperty("errores", out var errores)
                && errores.ValueKind == JsonValueKind.Array
                && errores.GetArrayLength() > 0)
            {
                return errores.EnumerateArray()
                    .Select(x => x.GetString() ?? "")
                    .Where(x => x.Length > 0)
                    .ToList();
            }

            var partes = new List<string>();
            if (sobre.TryGetProperty("mensaje", out var m)) partes.Add(m.GetString() ?? "");
            if (sobre.TryGetProperty("detalle", out var dt)) partes.Add(dt.GetString() ?? "");
            partes.RemoveAll(string.IsNullOrWhiteSpace);

            return partes.Count > 0
                ? partes
                : new List<string> { "No se pudo completar la operación." };
        }
        catch
        {
            return new List<string> { "No se pudo completar la operación." };
        }
    }
}
