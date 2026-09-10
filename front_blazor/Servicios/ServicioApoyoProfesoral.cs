using System.Net.Http.Json;
using System.Text.Json;

namespace FrontGestion.Servicios;

public class ApoyoProfesoral
{
    public int Estudios { get; set; }
    public byte ConApoyo { get; set; }
    public string Institucion { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
}

public class ServicioApoyoProfesoral
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

    public ServicioApoyoProfesoral(HttpClient http)
    {
        _http = http;
    }

    public async Task<Resultado<List<ApoyoProfesoral>>> Listar(
        int limite = 1000)
    {
        try
        {
            var r = await _http.GetAsync(
                $"/api/apoyo-profesoral?limite={limite}");

            if (r.StatusCode ==
                System.Net.HttpStatusCode.NoContent)
            {
                return Resultado<List<ApoyoProfesoral>>
                    .Bien(new());
            }

            if (!r.IsSuccessStatusCode)
            {
                return Resultado<List<ApoyoProfesoral>>
                    .Mal(await Mensajes(r));
            }

            var sobre =
                await r.Content.ReadFromJsonAsync<JsonElement>();

            var datos = sobre.GetProperty("datos")
                .Deserialize<List<ApoyoProfesoral>>(_opciones)
                ?? new();

            return Resultado<List<ApoyoProfesoral>>
                .Bien(datos);
        }
        catch (Exception e) when (
            e is HttpRequestException or TaskCanceledException)
        {
            return Resultado<List<ApoyoProfesoral>>
                .Mal(NoDisponible);
        }
    }

    public async Task<Resultado<bool>> Crear(
        ApoyoProfesoral entidad)
    {
        return await Enviar(
            HttpMethod.Post,
            "/api/apoyo-profesoral",
            entidad);
    }

    public async Task<Resultado<bool>> Reemplazar(
        int estudiosActual,
        ApoyoProfesoral entidad)
    {
        var cuerpo = new
        {
            estudios = entidad.Estudios,
            conApoyo = entidad.ConApoyo,
            institucion = entidad.Institucion,
            tipo = entidad.Tipo
        };

        return await Enviar(
            HttpMethod.Put,
            $"/api/apoyo-profesoral/{estudiosActual}",
            cuerpo);
    }

    public async Task<Resultado<bool>> Eliminar(
        int estudios)
    {
        return await Enviar(
            HttpMethod.Delete,
            $"/api/apoyo-profesoral/{estudios}",
            null);
    }

    private async Task<Resultado<bool>> Enviar(
        HttpMethod metodo,
        string ruta,
        object? cuerpo)
    {
        try
        {
            var peticion =
                new HttpRequestMessage(metodo, ruta);

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
