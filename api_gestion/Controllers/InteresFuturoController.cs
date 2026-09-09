using ApiGestion.Excepciones;
using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/intereses-futuros")]
public class InteresFuturoController : ControllerBase
{
    private readonly IServicioInteresFuturo _servicio;

    public InteresFuturoController(IServicioInteresFuturo servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(
        [FromQuery] int limite = 1000)
    {
        try
        {
            var datos = (await _servicio.ObtenerTodos(limite)).ToList();

            if (datos.Count == 0)
                return NoContent();

            return Ok(new
            {
                tabla = "intereses_futuros",
                limite,
                total = datos.Count,
                datos
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                estado = 400,
                mensaje = "Parámetros inválidos.",
                detalle = ex.Message
            });
        }
    }

    [HttpGet("{docente:int}/{terminoClave}")]
    public async Task<IActionResult> ObtenerPorId(
        int docente,
        string terminoClave)
    {
        try
        {
            return Ok(
                await _servicio.ObtenerPorId(docente, terminoClave));
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Interés futuro no encontrado.",
                detalle = ex.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] InteresFuturoCrear peticion)
    {
        try
        {
            await _servicio.Crear(peticion);

            return Ok(new
            {
                estado = 200,
                mensaje = "Interés futuro creado exitosamente."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                estado = 400,
                mensaje = "Parámetros inválidos.",
                detalle = ex.Message
            });
        }
        catch (SqlException ex)
        {
            return StatusCode(500, new
            {
                estado = 500,
                mensaje = "Error al insertar en la base de datos.",
                detalle = ex.Message
            });
        }
    }

    [HttpPut("{docente:int}/{terminoClave}")]
    public async Task<IActionResult> Reemplazar(
        int docente,
        string terminoClave,
        [FromBody] InteresFuturoReemplazo peticion)
    {
        try
        {
            var filas = await _servicio.Reemplazar(
                docente,
                terminoClave,
                peticion);

            return Ok(new
            {
                estado = 200,
                mensaje = "Interés futuro reemplazado.",
                filasAfectadas = filas
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                estado = 400,
                mensaje = "Parámetros inválidos.",
                detalle = ex.Message
            });
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Interés futuro no encontrado.",
                detalle = ex.Message
            });
        }
    }

    [HttpPatch("{docente:int}/{terminoClave}")]
    public async Task<IActionResult> ActualizarParcial(
        int docente,
        string terminoClave,
        [FromBody] InteresFuturoActualizar peticion)
    {
        try
        {
            var filas = await _servicio.ActualizarParcial(
                docente,
                terminoClave,
                peticion);

            return Ok(new
            {
                estado = 200,
                mensaje = "Interés futuro actualizado.",
                filasAfectadas = filas
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                estado = 400,
                mensaje = "Parámetros inválidos.",
                detalle = ex.Message
            });
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Interés futuro no encontrado.",
                detalle = ex.Message
            });
        }
    }

    [HttpDelete("{docente:int}/{terminoClave}")]
    public async Task<IActionResult> Eliminar(
        int docente,
        string terminoClave)
    {
        try
        {
            var filas = await _servicio.Eliminar(docente, terminoClave);

            return Ok(new
            {
                estado = 200,
                mensaje = "Interés futuro eliminado.",
                filasAfectadas = filas
            });
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Interés futuro no encontrado.",
                detalle = ex.Message
            });
        }
    }
}
