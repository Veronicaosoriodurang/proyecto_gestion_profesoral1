using System.Net.Http.Json;
using System.Text.Json;

namespace FrontGestion.Servicios;

public class DocenteDepartamento
{
    public int Docente { get; set; }
    public int Departamento { get; set; }
    public string Dedicacion { get; set; } = string.Empty;
    public string Modalidad { get; set; } = string.Empty;
    public DateOnly? FechaIngreso { get; set; }
    public DateOnly? FechaSalida { get; set; }
}

public class ServicioDocenteDepartamento
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

    public ServicioDocenteDepartamento(HttpClient http)
    {
        _http = http;
    }

    public async Task<Resultado<List<DocenteDepartamento>>> Listar(int limite = 1000)
    {
        try
        {
            var r = await _http.GetAsync(
                $"/api/docente-departamento?limite={limite}");

            if (r.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Resultado<List<DocenteDepartamento>>.Bien(new());

            if (!r.IsSuccessStatusCode)
                return Resultado<List<DocenteDepartamento>>.Mal(
                    await Mensajes(r));

            var sobre = await r.Content.ReadFromJsonAsync<JsonElement>();

            var datos = sobre.GetProperty("datos")
                .Deserialize<List<DocenteDepartamento>>(_opciones) ?? new();

            return Resultado<List<DocenteDepartamento>>.Bien(datos);
        }
        catch (Exception e) when (
            e is HttpRequestException or TaskCanceledException)
        {
            return Resultado<List<DocenteDepartamento>>.Mal(NoDisponible);
        }
    }

    public async Task<Resultado<bool>> Crear(DocenteDepartamento entidad)
    {
        return await Enviar(
            HttpMethod.Post,
            "/api/docente-departamento",
            entidad);
    }

    public async Task<Resultado<bool>> Reemplazar(
        int docente,
        int departamento,
        DocenteDepartamento entidad)
    {
        var cuerpo = new
        {
            dedicacion = entidad.Dedicacion,
            modalidad = entidad.Modalidad,
            fechaIngreso = entidad.FechaIngreso,
            fechaSalida = entidad.FechaSalida
        };

        return await Enviar(
            HttpMethod.Put,
            $"/api/docente-departamento/{docente}/{departamento}",
            cuerpo);
    }

    public async Task<Resultado<bool>> Eliminar(
        int docente,
        int departamento)
    {
        return await Enviar(
            HttpMethod.Delete,
            $"/api/docente-departamento/{docente}/{departamento}",
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
                && errores.ValueKind == JsonValueKind.Array)
            {
                return errores.EnumerateArray()
                    .Select(x => x.GetString() ?? "")
                    .Where(x => x.Length > 0)
                    .ToList();
            }

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
