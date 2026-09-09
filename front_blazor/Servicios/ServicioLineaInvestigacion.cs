using System.Net.Http.Json;
using System.Text.Json;

namespace FrontGestion.Servicios;

public class LineaInvestigacion
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}

public class ServicioLineaInvestigacion
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

    public ServicioLineaInvestigacion(HttpClient http)
    {
        _http = http;
    }

    public async Task<Resultado<List<LineaInvestigacion>>> Listar(int limite = 1000)
    {
        try
        {
            var r = await _http.GetAsync($"/api/linea-investigacion?limite={limite}");

            if (r.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Resultado<List<LineaInvestigacion>>.Bien(new());

            if (!r.IsSuccessStatusCode)
                return Resultado<List<LineaInvestigacion>>.Mal(await Mensajes(r));

            var sobre = await r.Content.ReadFromJsonAsync<JsonElement>();

            var datos = sobre.GetProperty("datos")
                .Deserialize<List<LineaInvestigacion>>(_opciones) ?? new();

            return Resultado<List<LineaInvestigacion>>.Bien(datos);
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            return Resultado<List<LineaInvestigacion>>.Mal(NoDisponible);
        }
    }

    public async Task<Resultado<LineaInvestigacion>> Obtener(int id)
    {
        try
        {
            var r = await _http.GetAsync($"/api/linea-investigacion/{id}");

            if (!r.IsSuccessStatusCode)
                return Resultado<LineaInvestigacion>.Mal(await Mensajes(r));

            var ficha = await r.Content.ReadFromJsonAsync<LineaInvestigacion>(_opciones);

            return Resultado<LineaInvestigacion>.Bien(ficha!);
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            return Resultado<LineaInvestigacion>.Mal(NoDisponible);
        }
    }

    public async Task<Resultado<bool>> Crear(LineaInvestigacion entidad)
    {
        var cuerpo = new
        {
            nombre = entidad.Nombre,
            descripcion = entidad.Descripcion
        };

        return await Enviar(
            HttpMethod.Post,
            "/api/linea-investigacion",
            cuerpo);
    }

    public async Task<Resultado<bool>> Reemplazar(
        int id,
        LineaInvestigacion entidad)
    {
        var cuerpo = new
        {
            nombre = entidad.Nombre,
            descripcion = entidad.Descripcion
        };

        return await Enviar(
            HttpMethod.Put,
            $"/api/linea-investigacion/{id}",
            cuerpo);
    }

    public async Task<Resultado<bool>> Actualizar(
        int id,
        string? nombre,
        string? descripcion)
    {
        var cuerpo = new Dictionary<string, object?>();

        if (!string.IsNullOrWhiteSpace(nombre))
            cuerpo["nombre"] = nombre;

        if (!string.IsNullOrWhiteSpace(descripcion))
            cuerpo["descripcion"] = descripcion;

        return await Enviar(
            HttpMethod.Patch,
            $"/api/linea-investigacion/{id}",
            cuerpo);
    }

    public async Task<Resultado<bool>> Eliminar(int id)
    {
        return await Enviar(
            HttpMethod.Delete,
            $"/api/linea-investigacion/{id}",
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
