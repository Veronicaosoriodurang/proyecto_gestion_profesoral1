using System.Net.Http.Json;
using System.Text.Json;

namespace FrontGestion.Servicios;

public class TerminoClave
{
    public string Termino { get; set; } = string.Empty;
    public string? TerminoIngles { get; set; }
}

public class ServicioTerminoClave
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

    public ServicioTerminoClave(HttpClient http)
    {
        _http = http;
    }

    public async Task<Resultado<List<TerminoClave>>> Listar(int limite = 1000)
    {
        try
        {
            var r = await _http.GetAsync($"/api/termino-clave?limite={limite}");

            if (r.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Resultado<List<TerminoClave>>.Bien(new());

            if (!r.IsSuccessStatusCode)
                return Resultado<List<TerminoClave>>.Mal(await Mensajes(r));

            var sobre = await r.Content.ReadFromJsonAsync<JsonElement>();

            var datos = sobre.GetProperty("datos")
                .Deserialize<List<TerminoClave>>(_opciones) ?? new();

            return Resultado<List<TerminoClave>>.Bien(datos);
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            return Resultado<List<TerminoClave>>.Mal(NoDisponible);
        }
    }

    public async Task<Resultado<TerminoClave>> Obtener(string termino)
    {
        try
        {
            var r = await _http.GetAsync(
                $"/api/termino-clave/{Uri.EscapeDataString(termino)}");

            if (!r.IsSuccessStatusCode)
                return Resultado<TerminoClave>.Mal(await Mensajes(r));

            var ficha = await r.Content.ReadFromJsonAsync<TerminoClave>(_opciones);

            return Resultado<TerminoClave>.Bien(ficha!);
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            return Resultado<TerminoClave>.Mal(NoDisponible);
        }
    }

    public async Task<Resultado<bool>> Crear(TerminoClave entidad)
    {
        return await Enviar(
            HttpMethod.Post,
            "/api/termino-clave",
            entidad);
    }

    public async Task<Resultado<bool>> Reemplazar(
        string termino,
        TerminoClave entidad)
    {
        var cuerpo = new
        {
            terminoIngles = entidad.TerminoIngles
        };

        return await Enviar(
            HttpMethod.Put,
            $"/api/termino-clave/{Uri.EscapeDataString(termino)}",
            cuerpo);
    }

    public async Task<Resultado<bool>> Actualizar(
        string termino,
        string? terminoIngles)
    {
        var cuerpo = new Dictionary<string, object?>();

        if (!string.IsNullOrWhiteSpace(terminoIngles))
            cuerpo["terminoIngles"] = terminoIngles;

        return await Enviar(
            HttpMethod.Patch,
            $"/api/termino-clave/{Uri.EscapeDataString(termino)}",
            cuerpo);
    }

    public async Task<Resultado<bool>> Eliminar(string termino)
    {
        return await Enviar(
            HttpMethod.Delete,
            $"/api/termino-clave/{Uri.EscapeDataString(termino)}",
            null);
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
