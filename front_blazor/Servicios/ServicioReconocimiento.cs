using System.Net.Http.Json;
using System.Text.Json;

namespace FrontGestion.Servicios;

public class Reconocimiento
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public DateOnly? Fecha { get; set; }
    public string Institucion { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Ambito { get; set; } = string.Empty;
    public int Docente { get; set; }
}

public class ServicioReconocimiento
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

    public ServicioReconocimiento(HttpClient http)
    {
        _http = http;
    }

    public async Task<Resultado<List<Reconocimiento>>> Listar(int limite = 1000)
    {
        try
        {
            var r = await _http.GetAsync(
                $"/api/reconocimiento?limite={limite}");

            if (r.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Resultado<List<Reconocimiento>>.Bien(new());

            if (!r.IsSuccessStatusCode)
                return Resultado<List<Reconocimiento>>.Mal(await Mensajes(r));

            var sobre = await r.Content.ReadFromJsonAsync<JsonElement>();

            var datos = sobre.GetProperty("datos")
                .Deserialize<List<Reconocimiento>>(_opciones) ?? new();

            return Resultado<List<Reconocimiento>>.Bien(datos);
        }
        catch (Exception e) when (
            e is HttpRequestException or TaskCanceledException)
        {
            return Resultado<List<Reconocimiento>>.Mal(NoDisponible);
        }
    }

    public async Task<Resultado<bool>> Crear(Reconocimiento entidad)
    {
        var cuerpo = new
        {
            tipo = entidad.Tipo,
            fecha = entidad.Fecha,
            institucion = entidad.Institucion,
            nombre = entidad.Nombre,
            ambito = entidad.Ambito,
            docente = entidad.Docente
        };

        return await Enviar(
            HttpMethod.Post,
            "/api/reconocimiento",
            cuerpo);
    }

    public async Task<Resultado<bool>> Reemplazar(
        int id,
        Reconocimiento entidad)
    {
        var cuerpo = new
        {
            tipo = entidad.Tipo,
            fecha = entidad.Fecha,
            institucion = entidad.Institucion,
            nombre = entidad.Nombre,
            ambito = entidad.Ambito,
            docente = entidad.Docente
        };

        return await Enviar(
            HttpMethod.Put,
            $"/api/reconocimiento/{id}",
            cuerpo);
    }

    public async Task<Resultado<bool>> Eliminar(int id)
    {
        return await Enviar(
            HttpMethod.Delete,
            $"/api/reconocimiento/{id}",
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
