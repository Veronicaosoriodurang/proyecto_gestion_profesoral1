using ApiGestion.Excepciones;
using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/red-docente")]
public class RedDocenteController : ControllerBase
{
    private readonly IServicioRedDocente _servicio;

    public RedDocenteController(
        IServicioRedDocente servicio)
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
                tabla = "red_docente",
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

    [HttpGet("{red:int}/{docente:int}")]
    public async Task<IActionResult> ObtenerPorId(
        int red,
        int docente)
    {
        try
        {
            return Ok(
                await _servicio.ObtenerPorId(
                    red,
                    docente));
        }
        catch (NoEncontradoExcepcion e)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Relación red-docente no encontrada.",
                detalle = e.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] RedDocenteCrear peticion)
    {
        try
        {
            var filas =
                await _servicio.Crear(peticion);

            return Ok(new
            {
                estado = 200,
                mensaje = "Relación red-docente creada exitosamente.",
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

    [HttpPut("{red:int}/{docente:int}")]
    public async Task<IActionResult> Reemplazar(
        int red,
        int docente,
        [FromBody] RedDocenteReemplazo peticion)
    {
        try
        {
            var filas =
                await _servicio.Reemplazar(
                    red,
                    docente,
                    peticion);

            return Ok(new
            {
                estado = 200,
                mensaje = "Relación red-docente reemplazada.",
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
                mensaje = "Relación red-docente no encontrada.",
                detalle = e.Message
            });
        }
    }

    [HttpPatch("{red:int}/{docente:int}")]
    public async Task<IActionResult> Actualizar(
        int red,
        int docente,
        [FromBody] RedDocenteActualizar peticion)
    {
        try
        {
            var filas =
                await _servicio.ActualizarParcial(
                    red,
                    docente,
                    peticion);

            return Ok(new
            {
                estado = 200,
                mensaje = "Relación red-docente actualizada.",
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
                mensaje = "Relación red-docente no encontrada.",
                detalle = e.Message
            });
        }
    }

    [HttpDelete("{red:int}/{docente:int}")]
    public async Task<IActionResult> Eliminar(
        int red,
        int docente)
    {
        try
        {
            var filas =
                await _servicio.Eliminar(
                    red,
                    docente);

            return Ok(new
            {
                estado = 200,
                mensaje = "Relación red-docente eliminada.",
                filasAfectadas = filas
            });
        }
        catch (NoEncontradoExcepcion e)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Relación red-docente no encontrada.",
                detalle = e.Message
            });
        }
    }
}
