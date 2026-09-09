using ApiGestion.Excepciones;
using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/estudio-ac")]
public class EstudioAcController : ControllerBase
{
    private readonly IServicioEstudioAc _servicio;

    public EstudioAcController(
        IServicioEstudioAc servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(
        [FromQuery] int limite = 1000)
    {
        try
        {
            var datos =
                (await _servicio.ObtenerTodos(limite))
                .ToList();

            if (datos.Count == 0)
                return NoContent();

            return Ok(new
            {
                tabla = "estudio_ac",
                limite,
                total = datos.Count,
                datos
            });
        }
        catch (ArgumentException e)
        {
            return BadRequest(new
            {
                estado = 400,
                mensaje = "Parámetros inválidos.",
                detalle = e.Message
            });
        }
    }

    [HttpGet("{estudio:int}/{areaConocimiento}")]
    public async Task<IActionResult> ObtenerPorId(
        int estudio,
        string areaConocimiento)
    {
        try
        {
            return Ok(
                await _servicio.ObtenerPorId(
                    estudio,
                    areaConocimiento));
        }
        catch (NoEncontradoExcepcion e)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Relación estudio-área no encontrada.",
                detalle = e.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] EstudioAcCrear peticion)
    {
        try
        {
            var filas = await _servicio.Crear(peticion);

            return Ok(new
            {
                estado = 200,
                mensaje = "Relación estudio-área creada exitosamente.",
                filasAfectadas = filas
            });
        }
        catch (ArgumentException e)
        {
            return BadRequest(new
            {
                estado = 400,
                mensaje = "Parámetros inválidos.",
                detalle = e.Message
            });
        }
    }

    [HttpPut("{estudio:int}/{areaConocimiento}")]
    public async Task<IActionResult> Reemplazar(
        int estudio,
        string areaConocimiento,
        [FromBody] EstudioAcReemplazo peticion)
    {
        try
        {
            var filas =
                await _servicio.Reemplazar(
                    estudio,
                    areaConocimiento,
                    peticion);

            return Ok(new
            {
                estado = 200,
                mensaje = "Relación estudio-área reemplazada.",
                filasAfectadas = filas
            });
        }
        catch (ArgumentException e)
        {
            return BadRequest(new
            {
                estado = 400,
                mensaje = "Parámetros inválidos.",
                detalle = e.Message
            });
        }
        catch (NoEncontradoExcepcion e)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Relación estudio-área no encontrada.",
                detalle = e.Message
            });
        }
    }

    [HttpPatch("{estudio:int}/{areaConocimiento}")]
    public async Task<IActionResult> Actualizar(
        int estudio,
        string areaConocimiento,
        [FromBody] EstudioAcActualizar peticion)
    {
        try
        {
            var filas =
                await _servicio.ActualizarParcial(
                    estudio,
                    areaConocimiento,
                    peticion);

            return Ok(new
            {
                estado = 200,
                mensaje = "Relación estudio-área actualizada.",
                filasAfectadas = filas
            });
        }
        catch (ArgumentException e)
        {
            return BadRequest(new
            {
                estado = 400,
                mensaje = "Parámetros inválidos.",
                detalle = e.Message
            });
        }
        catch (NoEncontradoExcepcion e)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Relación estudio-área no encontrada.",
                detalle = e.Message
            });
        }
    }

    [HttpDelete("{estudio:int}/{areaConocimiento}")]
    public async Task<IActionResult> Eliminar(
        int estudio,
        string areaConocimiento)
    {
        try
        {
            var filas =
                await _servicio.Eliminar(
                    estudio,
                    areaConocimiento);

            return Ok(new
            {
                estado = 200,
                mensaje = "Relación estudio-área eliminada.",
                filasAfectadas = filas
            });
        }
        catch (NoEncontradoExcepcion e)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Relación estudio-área no encontrada.",
                detalle = e.Message
            });
        }
    }
}
