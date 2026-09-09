using System.Net.Http.Json;
using System.Text.Json;

namespace FrontGestion.Servicios;

public class AreaConocimiento
{
    public string Id { get; set; } = string.Empty;
    public string GranArea { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Disciplina { get; set; } = string.Empty;
}

public class ServicioAreaConocimiento
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

    public ServicioAreaConocimiento(HttpClient http)
    {
        _http = http;
    }

    public async Task<Resultado<List<AreaConocimiento>>> Listar(int limite = 1000)
    {
        try
        {
            var r = await _http.GetAsync($"/api/area-conocimiento?limite={limite}");

            if (r.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Resultado<List<AreaConocimiento>>.Bien(new());

            if (!r.IsSuccessStatusCode)
                return Resultado<List<AreaConocimiento>>.Mal(await Mensajes(r));

            var sobre = await r.Content.ReadFromJsonAsync<JsonElement>();

            var datos = sobre.GetProperty("datos")
                .Deserialize<List<AreaConocimiento>>(_opciones) ?? new();

            return Resultado<List<AreaConocimiento>>.Bien(datos);
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            return Resultado<List<AreaConocimiento>>.Mal(NoDisponible);
        }
    }

    public async Task<Resultado<AreaConocimiento>> Obtener(string id)
    {
        try
        {
            var r = await _http.GetAsync($"/api/area-conocimiento/{id}");

            if (!r.IsSuccessStatusCode)
                return Resultado<AreaConocimiento>.Mal(await Mensajes(r));

            var ficha = await r.Content.ReadFromJsonAsync<AreaConocimiento>(_opciones);

            return Resultado<AreaConocimiento>.Bien(ficha!);
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            return Resultado<AreaConocimiento>.Mal(NoDisponible);
        }
    }

    public async Task<Resultado<bool>> Crear(AreaConocimiento entidad)
    {
        return await Enviar(HttpMethod.Post, "/api/area-conocimiento", entidad);
    }

    public async Task<Resultado<bool>> Reemplazar(string id, AreaConocimiento entidad)
    {
        var cuerpo = new
        {
            granArea = entidad.GranArea,
            area = entidad.Area,
            disciplina = entidad.Disciplina
        };

        return await Enviar(HttpMethod.Put, $"/api/area-conocimiento/{id}", cuerpo);
    }

    public async Task<Resultado<bool>> Actualizar(
        string id,
        string? granArea,
        string? area,
        string? disciplina)
    {
        var cuerpo = new Dictionary<string, object?>();

        if (!string.IsNullOrWhiteSpace(granArea))
            cuerpo["granArea"] = granArea;

        if (!string.IsNullOrWhiteSpace(area))
            cuerpo["area"] = area;

        if (!string.IsNullOrWhiteSpace(disciplina))
            cuerpo["disciplina"] = disciplina;

        return await Enviar(HttpMethod.Patch, $"/api/area-conocimiento/{id}", cuerpo);
    }

    public async Task<Resultado<bool>> Eliminar(string id)
    {
        return await Enviar(HttpMethod.Delete, $"/api/area-conocimiento/{id}", null);
    }

    private async Task<Resultado<bool>> Enviar(
        HttpMethod metodo,
        string ruta,
        object? cuerpo)
    {
        try
        {
            var peticion = new HttpRequestMessage(metodo, ruta);

            if (cuerpo != null)
                peticion.Content = JsonContent.Create(cuerpo);

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

            if (sobre.TryGetProperty("mensaje", out var m))
                partes.Add(m.GetString() ?? "");

            if (sobre.TryGetProperty("detalle", out var dt))
                partes.Add(dt.GetString() ?? "");

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
