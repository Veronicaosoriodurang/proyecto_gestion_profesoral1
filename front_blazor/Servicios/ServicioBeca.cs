using System.Net.Http.Json;
using System.Text.Json;

namespace FrontGestion.Servicios;

public class Beca
{
    public int Estudios { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Institucion { get; set; } = string.Empty;
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
}

public class ServicioBeca
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

    public ServicioBeca(HttpClient http)
    {
        _http = http;
    }

    public async Task<Resultado<List<Beca>>> Listar(int limite = 1000)
    {
        try
        {
            var r = await _http.GetAsync(
                $"/api/beca?limite={limite}");

            if (r.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Resultado<List<Beca>>.Bien(new());

            if (!r.IsSuccessStatusCode)
                return Resultado<List<Beca>>.Mal(await Mensajes(r));

            var sobre = await r.Content.ReadFromJsonAsync<JsonElement>();

            var datos = sobre.GetProperty("datos")
                .Deserialize<List<Beca>>(_opciones) ?? new();

            return Resultado<List<Beca>>.Bien(datos);
        }
        catch (Exception e) when (
            e is HttpRequestException or TaskCanceledException)
        {
            return Resultado<List<Beca>>.Mal(NoDisponible);
        }
    }

    public async Task<Resultado<bool>> Crear(Beca entidad)
    {
        return await Enviar(
            HttpMethod.Post,
            "/api/beca",
            entidad);
    }

    public async Task<Resultado<bool>> Reemplazar(
        int estudiosActual,
        Beca entidad)
    {
        var cuerpo = new
        {
            estudios = entidad.Estudios,
            tipo = entidad.Tipo,
            institucion = entidad.Institucion,
            fechaInicio = entidad.FechaInicio,
            fechaFin = entidad.FechaFin
        };

        return await Enviar(
            HttpMethod.Put,
            $"/api/beca/{estudiosActual}",
            cuerpo);
    }

    public async Task<Resultado<bool>> Eliminar(int estudios)
    {
        return await Enviar(
            HttpMethod.Delete,
            $"/api/beca/{estudios}",
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
            var sobre = await r.Content.ReadFromJsonAsync<JsonElement>();

            var partes = new List<string>();

            if (sobre.TryGetProperty("mensaje", out var m))
                partes.Add(m.GetString() ?? "");

            if (sobre.TryGetProperty("detalle", out var d))
                partes.Add(d.GetString() ?? "");

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
