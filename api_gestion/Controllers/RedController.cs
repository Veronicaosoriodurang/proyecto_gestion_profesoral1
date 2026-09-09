using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/red")]
public class RedController : ControllerBase
{
    private readonly IServicioRed _servicio;

    public RedController(IServicioRed servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos([FromQuery] int limite = 1000)
    {
        try
        {
            var datos = await _servicio.ObtenerTodos(limite);

            return Ok(new
            {
                tabla = "red",
                limite,
                total = datos.Count(),
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
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                estado = 500,
                mensaje = "Error interno del servidor.",
                detalle = ex.Message
            });
        }
    }

    [HttpGet("{idr:int}")]
    public async Task<IActionResult> ObtenerPorId(int idr)
    {
        try
        {
            return Ok(await _servicio.ObtenerPorId(idr));
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Red no encontrada.",
                detalle = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                estado = 500,
                mensaje = "Error interno del servidor.",
                detalle = ex.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] RedCrear peticion)
    {
        try
        {
            var red = new Red
            {
                Idr = peticion.Idr!.Value,
                Nombre = peticion.Nombre,
                Url = peticion.Url,
                Pais = peticion.Pais
            };

            await _servicio.Crear(red);

            return Ok(new
            {
                estado = 200,
                mensaje = "Red creada exitosamente."
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
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                estado = 500,
                mensaje = "Error interno del servidor.",
                detalle = ex.Message
            });
        }
    }

    [HttpPut("{idr:int}")]
    public async Task<IActionResult> Reemplazar(
        int idr,
        [FromBody] RedReemplazo peticion)
    {
        try
        {
            var red = new Red
            {
                Idr = idr,
                Nombre = peticion.Nombre,
                Url = peticion.Url,
                Pais = peticion.Pais
            };

            var filas = await _servicio.Reemplazar(idr, red);

            return Ok(new
            {
                estado = 200,
                mensaje = "Red reemplazada.",
                filasAfectadas = filas
            });
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Red no encontrada.",
                detalle = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                estado = 500,
                mensaje = "Error interno del servidor.",
                detalle = ex.Message
            });
        }
    }

    [HttpPatch("{idr:int}")]
    public async Task<IActionResult> ActualizarParcial(
        int idr,
        [FromBody] RedActualizar peticion)
    {
        try
        {
            var campos = new RedCampos(
                peticion.Nombre,
                peticion.Url,
                peticion.Pais);

            var filas = await _servicio.ActualizarParcial(idr, campos);

            return Ok(new
            {
                estado = 200,
                mensaje = "Red actualizada.",
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
                mensaje = "Red no encontrada.",
                detalle = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                estado = 500,
                mensaje = "Error interno del servidor.",
                detalle = ex.Message
            });
        }
    }

    [HttpDelete("{idr:int}")]
    public async Task<IActionResult> Eliminar(int idr)
    {
        try
        {
            var filas = await _servicio.Eliminar(idr);

            return Ok(new
            {
                estado = 200,
                mensaje = "Red eliminada.",
                filasAfectadas = filas
            });
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Red no encontrada.",
                detalle = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                estado = 500,
                mensaje = "Error interno del servidor.",
                detalle = ex.Message
            });
        }
    }
}
