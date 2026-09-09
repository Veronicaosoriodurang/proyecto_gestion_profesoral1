using System.Net.Http.Json;
using System.Text.Json;

namespace FrontGestion.Servicios;

public class InteresFuturo
{
    public int Docente { get; set; }
    public string TerminoClave { get; set; } = string.Empty;
}

public class ServicioInteresFuturo
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

    public ServicioInteresFuturo(HttpClient http)
    {
        _http = http;
    }

    public async Task<Resultado<List<InteresFuturo>>> Listar(int limite = 1000)
    {
        try
        {
            var r = await _http.GetAsync(
                $"/api/intereses-futuros?limite={limite}");

            if (r.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Resultado<List<InteresFuturo>>.Bien(new());

            if (!r.IsSuccessStatusCode)
                return Resultado<List<InteresFuturo>>.Mal(await Mensajes(r));

            var sobre = await r.Content.ReadFromJsonAsync<JsonElement>();

            var datos = sobre.GetProperty("datos")
                .Deserialize<List<InteresFuturo>>(_opciones) ?? new();

            return Resultado<List<InteresFuturo>>.Bien(datos);
        }
        catch (Exception e) when (
            e is HttpRequestException or TaskCanceledException)
        {
            return Resultado<List<InteresFuturo>>.Mal(NoDisponible);
        }
    }

    public async Task<Resultado<bool>> Crear(InteresFuturo entidad)
    {
        var cuerpo = new
        {
            docente = entidad.Docente,
            terminoClave = entidad.TerminoClave
        };

        return await Enviar(
            HttpMethod.Post,
            "/api/intereses-futuros",
            cuerpo);
    }

    public async Task<Resultado<bool>> Reemplazar(
        int docenteActual,
        string terminoActual,
        InteresFuturo entidad)
    {
        var cuerpo = new
        {
            docente = entidad.Docente,
            terminoClave = entidad.TerminoClave
        };

        return await Enviar(
            HttpMethod.Put,
            $"/api/intereses-futuros/{docenteActual}/{Uri.EscapeDataString(terminoActual)}",
            cuerpo);
    }

    public async Task<Resultado<bool>> Eliminar(
        int docente,
        string terminoClave)
    {
        return await Enviar(
            HttpMethod.Delete,
            $"/api/intereses-futuros/{docente}/{Uri.EscapeDataString(terminoClave)}",
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
        catch (Exception e) when (
            e is HttpRequestException or TaskCanceledException)
        {
            return Resultado<bool>.Mal(NoDisponible);
        }
    }

    private static async Task<List<string>> Mensajes(
        HttpResponseMessage r)
    {
        try
        {
            var sobre =
                await r.Content.ReadFromJsonAsync<JsonElement>();

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
                : new List<string>
                {
                    "No se pudo completar la operación."
                };
        }
        catch
        {
            return new List<string>
            {
                "No se pudo completar la operación."
            };
        }
    }
}
