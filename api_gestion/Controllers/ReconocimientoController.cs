using ApiGestion.Excepciones;
using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/reconocimiento")]
public class ReconocimientoController : ControllerBase
{
    private readonly IServicioReconocimiento _servicio;

    public ReconocimientoController(IServicioReconocimiento servicio)
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
                tabla = "reconocimiento",
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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        try
        {
            return Ok(await _servicio.ObtenerPorId(id));
        }
        catch (NoEncontradoExcepcion e)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Reconocimiento no encontrado.",
                detalle = e.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] ReconocimientoCrear peticion)
    {
        try
        {
            var id = await _servicio.Crear(peticion);

            return Ok(new
            {
                estado = 200,
                mensaje = "Reconocimiento creado exitosamente.",
                id
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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Reemplazar(
        int id,
        [FromBody] ReconocimientoReemplazo peticion)
    {
        try
        {
            var filas = await _servicio.Reemplazar(id, peticion);

            return Ok(new
            {
                estado = 200,
                mensaje = "Reconocimiento reemplazado.",
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
                mensaje = "Reconocimiento no encontrado.",
                detalle = e.Message
            });
        }
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] ReconocimientoActualizar peticion)
    {
        try
        {
            var filas =
                await _servicio.ActualizarParcial(id, peticion);

            return Ok(new
            {
                estado = 200,
                mensaje = "Reconocimiento actualizado.",
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
                mensaje = "Reconocimiento no encontrado.",
                detalle = e.Message
            });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            var filas = await _servicio.Eliminar(id);

            return Ok(new
            {
                estado = 200,
                mensaje = "Reconocimiento eliminado.",
                filasAfectadas = filas
            });
        }
        catch (NoEncontradoExcepcion e)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Reconocimiento no encontrado.",
                detalle = e.Message
            });
        }
    }
}
