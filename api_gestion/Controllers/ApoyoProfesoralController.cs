using ApiGestion.Excepciones;
using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/apoyo-profesoral")]
public class ApoyoProfesoralController : ControllerBase
{
    private readonly IServicioApoyoProfesoral _servicio;

    public ApoyoProfesoralController(
        IServicioApoyoProfesoral servicio)
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
                tabla = "apoyo_profesoral",
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

    [HttpGet("{estudios:int}")]
    public async Task<IActionResult> ObtenerPorId(
        int estudios)
    {
        try
        {
            return Ok(
                await _servicio.ObtenerPorId(estudios));
        }
        catch (NoEncontradoExcepcion e)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Apoyo profesoral no encontrado.",
                detalle = e.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] ApoyoProfesoralCrear peticion)
    {
        try
        {
            var filas = await _servicio.Crear(peticion);

            return Ok(new
            {
                estado = 200,
                mensaje = "Apoyo profesoral creado exitosamente.",
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

    [HttpPut("{estudios:int}")]
    public async Task<IActionResult> Reemplazar(
        int estudios,
        [FromBody] ApoyoProfesoralReemplazo peticion)
    {
        try
        {
            var filas = await _servicio.Reemplazar(
                estudios,
                peticion);

            return Ok(new
            {
                estado = 200,
                mensaje = "Apoyo profesoral reemplazado.",
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
                mensaje = "Apoyo profesoral no encontrado.",
                detalle = e.Message
            });
        }
    }

    [HttpPatch("{estudios:int}")]
    public async Task<IActionResult> Actualizar(
        int estudios,
        [FromBody] ApoyoProfesoralActualizar peticion)
    {
        try
        {
            var filas = await _servicio.ActualizarParcial(
                estudios,
                peticion);

            return Ok(new
            {
                estado = 200,
                mensaje = "Apoyo profesoral actualizado.",
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
                mensaje = "Apoyo profesoral no encontrado.",
                detalle = e.Message
            });
        }
    }

    [HttpDelete("{estudios:int}")]
    public async Task<IActionResult> Eliminar(int estudios)
    {
        try
        {
            var filas = await _servicio.Eliminar(estudios);

            return Ok(new
            {
                estado = 200,
                mensaje = "Apoyo profesoral eliminado.",
                filasAfectadas = filas
            });
        }
        catch (NoEncontradoExcepcion e)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Apoyo profesoral no encontrado.",
                detalle = e.Message
            });
        }
    }
}
