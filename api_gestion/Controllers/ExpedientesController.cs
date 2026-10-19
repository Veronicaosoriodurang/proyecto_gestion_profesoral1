using ApiGestion.Excepciones;
using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
namespace ApiGestion.Controllers;

[ApiController]
[Route("api")]
public class ExpedientesController(IServicioExpedientes servicio) : ControllerBase
{
    [HttpPost("docente/expediente")]
    public Task<IActionResult> CrearDocente(ExpedienteDocenteCrear p) => Ejecutar(async () => await servicio.GuardarDocente(null, p));

    [HttpPost("docente/{cedula:int}/detalles")]
    public Task<IActionResult> AgregarDocente(int cedula, ExpedienteDocenteCrear p) => Ejecutar(async () => await servicio.GuardarDocente(cedula, p));

    [HttpPost("estudios-realizados/expediente")]
    public Task<IActionResult> CrearEstudio(ExpedienteEstudioCrear p) => Ejecutar(async () => await servicio.GuardarEstudio(null, p));

    [HttpPost("estudios-realizados/{id:int}/detalles")]
    public Task<IActionResult> AgregarEstudio(int id, ExpedienteEstudioCrear p) => Ejecutar(async () => await servicio.GuardarEstudio(id, p));

    [HttpGet("docente/{cedula:int}/expediente")]
    public Task<IActionResult> ObtenerDocente(int cedula) => Ejecutar(async () => await servicio.ObtenerDocente(cedula));
    [HttpGet("estudios-realizados/{id:int}/expediente")]
    public Task<IActionResult> ObtenerEstudio(int id) => Ejecutar(async () => await servicio.ObtenerEstudio(id));

    private async Task<IActionResult> Ejecutar(Func<Task<object>> operacion)
    {
        try { return Ok(new { estado = 200, mensaje = "Expediente guardado completo.", datos = await operacion() }); }
        catch (SqlException ex) when (ex.Number is 547 or 2601 or 2627 or 51001 or 51002)
        { return Conflict(new { estado = 409, mensaje = "No se guardó ningún renglón: revise relaciones activas y registros duplicados." }); }
        catch (NoEncontradoExcepcion ex) { return NotFound(new { estado = 404, mensaje = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { estado = 400, mensaje = ex.Message }); }
    }
}
