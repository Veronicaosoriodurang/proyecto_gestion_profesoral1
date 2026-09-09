using System.Net.Http.Json;
using System.Text.Json;

namespace FrontGestion.Servicios;

public class EstudioRealizado
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Universidad { get; set; } = string.Empty;
    public DateOnly? Fecha { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public int Docente { get; set; }
    public byte InsAcreditada { get; set; }
    public string Metodologia { get; set; } = string.Empty;
    public string PerfilEgresado { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
}

public class ServicioEstudioRealizado
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

    public ServicioEstudioRealizado(HttpClient http)
    {
        _http = http;
    }

    public async Task<Resultado<List<EstudioRealizado>>> Listar(int limite = 1000)
    {
        try
        {
            var r = await _http.GetAsync($"/api/estudios-realizados?limite={limite}");

            if (r.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Resultado<List<EstudioRealizado>>.Bien(new());

            if (!r.IsSuccessStatusCode)
                return Resultado<List<EstudioRealizado>>.Mal(await Mensajes(r));

            var sobre = await r.Content.ReadFromJsonAsync<JsonElement>();

            var datos = sobre.GetProperty("datos")
                .Deserialize<List<EstudioRealizado>>(_opciones) ?? new();

            return Resultado<List<EstudioRealizado>>.Bien(datos);
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            return Resultado<List<EstudioRealizado>>.Mal(NoDisponible);
        }
    }

    public async Task<Resultado<bool>> Crear(EstudioRealizado entidad)
    {
        return await Enviar(HttpMethod.Post, "/api/estudios-realizados", entidad);
    }

    public async Task<Resultado<bool>> Reemplazar(int id, EstudioRealizado entidad)
    {
        var cuerpo = new
        {
            titulo = entidad.Titulo,
            universidad = entidad.Universidad,
            fecha = entidad.Fecha,
            tipo = entidad.Tipo,
            ciudad = entidad.Ciudad,
            docente = entidad.Docente,
            insAcreditada = entidad.InsAcreditada,
            metodologia = entidad.Metodologia,
            perfilEgresado = entidad.PerfilEgresado,
            pais = entidad.Pais
        };

        return await Enviar(
            HttpMethod.Put,
            $"/api/estudios-realizados/{id}",
            cuerpo);
    }

    public async Task<Resultado<bool>> Eliminar(int id)
    {
        return await Enviar(
            HttpMethod.Delete,
            $"/api/estudios-realizados/{id}",
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

            var partes = new List<string>();

            if (sobre.TryGetProperty("mensaje", out var m))
                partes.Add(m.GetString() ?? "");

            if (sobre.TryGetProperty("detalle", out var d))
                partes.Add(d.GetString() ?? "");

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
