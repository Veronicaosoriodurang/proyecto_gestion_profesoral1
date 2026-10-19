using System.Net.Http.Json;
using System.Text.Json;
namespace FrontGestion.Servicios;

public class ServicioExpedientes(HttpClient http)
{
    private static readonly JsonSerializerOptions Opciones = new(JsonSerializerDefaults.Web);
    public async Task<Resultado<ExpedienteDocenteVista>> ObtenerDocente(int clave)
    {
        try
        {
            using var respuesta = await http.GetAsync($"/api/docente/{clave}/expediente");
            var json = await respuesta.Content.ReadFromJsonAsync<JsonElement>();
            return respuesta.IsSuccessStatusCode
                ? Resultado<ExpedienteDocenteVista>.Bien(json.GetProperty("datos").Deserialize<ExpedienteDocenteVista>(Opciones)!)
                : Resultado<ExpedienteDocenteVista>.Mal(Mensajes(json));
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        { return Resultado<ExpedienteDocenteVista>.Mal(new() { "No se pudo consultar el expediente." }); }
    }
    public Task<Resultado<bool>> GuardarDocente(Docente maestro, DetallesDocenteVista detalles, bool nuevo) =>
        Enviar(nuevo ? "/api/docente/expediente" : $"/api/docente/{maestro.Cedula}/detalles", new ExpedienteDocenteCrear { Docente = maestro, Detalles = detalles });
    public async Task<Resultado<ExpedienteEstudioVista>> ObtenerEstudio(int clave)
    {
        try
        {
            using var respuesta = await http.GetAsync($"/api/estudios-realizados/{clave}/expediente");
            var json = await respuesta.Content.ReadFromJsonAsync<JsonElement>();
            return respuesta.IsSuccessStatusCode
                ? Resultado<ExpedienteEstudioVista>.Bien(json.GetProperty("datos").Deserialize<ExpedienteEstudioVista>(Opciones)!)
                : Resultado<ExpedienteEstudioVista>.Mal(Mensajes(json));
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        { return Resultado<ExpedienteEstudioVista>.Mal(new() { "No se pudo consultar el expediente." }); }
    }
    public Task<Resultado<bool>> GuardarEstudio(EstudioRealizado maestro, DetallesEstudioVista detalles, bool nuevo) =>
        Enviar(nuevo ? "/api/estudios-realizados/expediente" : $"/api/estudios-realizados/{maestro.Id}/detalles", new ExpedienteEstudioCrear { Estudio = maestro, Detalles = detalles });
    private async Task<Resultado<bool>> Enviar<T>(string ruta, T cuerpo)
    {
        try
        {
            using var respuesta = await http.PostAsJsonAsync(ruta, cuerpo);
            if (respuesta.IsSuccessStatusCode) return Resultado<bool>.Bien(true);
            var json = await respuesta.Content.ReadFromJsonAsync<JsonElement>();
            return Resultado<bool>.Mal(Mensajes(json));
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        { return Resultado<bool>.Mal(new() { "No se pudo guardar el expediente. Consulte antes de reintentar." }); }
    }
    private static List<string> Mensajes(JsonElement json)
    {
        var lista = new List<string>();
        if (json.TryGetProperty("mensaje", out var mensaje)) lista.Add(mensaje.GetString() ?? "No se pudo completar la operación.");
        if (json.TryGetProperty("errores", out var errores)) lista.AddRange(errores.EnumerateArray().Select(x => x.GetString() ?? "Revise los datos."));
        return lista;
    }
}
