using System.Net.Http.Json;
using System.Text.Json;

namespace FrontGestion.Servicios;

public class Red
{
    public int Idr { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
}

public class ServicioRed
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

    public ServicioRed(HttpClient http)
    {
        _http = http;
    }

    public async Task<Resultado<List<Red>>> Listar(int limite = 1000)
    {
        try
        {
            var r = await _http.GetAsync($"/api/red?limite={limite}");

            if (r.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Resultado<List<Red>>.Bien(new());

            if (!r.IsSuccessStatusCode)
                return Resultado<List<Red>>.Mal(await Mensajes(r));

            var sobre = await r.Content.ReadFromJsonAsync<JsonElement>();

            var datos = sobre.GetProperty("datos")
                .Deserialize<List<Red>>(_opciones) ?? new();

            return Resultado<List<Red>>.Bien(datos);
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            return Resultado<List<Red>>.Mal(NoDisponible);
        }
    }

    public async Task<Resultado<Red>> Obtener(int idr)
    {
        try
        {
            var r = await _http.GetAsync($"/api/red/{idr}");

            if (!r.IsSuccessStatusCode)
                return Resultado<Red>.Mal(await Mensajes(r));

            var ficha = await r.Content.ReadFromJsonAsync<Red>(_opciones);

            return Resultado<Red>.Bien(ficha!);
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            return Resultado<Red>.Mal(NoDisponible);
        }
    }

    public async Task<Resultado<bool>> Crear(Red entidad)
    {
        return await Enviar(HttpMethod.Post, "/api/red", entidad);
    }

    public async Task<Resultado<bool>> Reemplazar(int idr, Red entidad)
    {
        var cuerpo = new
        {
            nombre = entidad.Nombre,
            url = entidad.Url,
            pais = entidad.Pais
        };

        return await Enviar(HttpMethod.Put, $"/api/red/{idr}", cuerpo);
    }

    public async Task<Resultado<bool>> Actualizar(
        int idr,
        string? nombre,
        string? url,
        string? pais)
    {
        var cuerpo = new Dictionary<string, object?>();

        if (!string.IsNullOrWhiteSpace(nombre))
            cuerpo["nombre"] = nombre;

        if (!string.IsNullOrWhiteSpace(url))
            cuerpo["url"] = url;

        if (!string.IsNullOrWhiteSpace(pais))
            cuerpo["pais"] = pais;

        return await Enviar(HttpMethod.Patch, $"/api/red/{idr}", cuerpo);
    }

    public async Task<Resultado<bool>> Eliminar(int idr)
    {
        return await Enviar(HttpMethod.Delete, $"/api/red/{idr}", null);
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
