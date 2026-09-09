using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/termino-clave")]
public class TerminoClaveController : ControllerBase
{
    private readonly IServicioTerminoClave _servicio;

    public TerminoClaveController(IServicioTerminoClave servicio)
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
                tabla = "termino_clave",
                limite,
                total = datos.Count(),
                datos
            });
        }
        catch (ArgumentException e)
        {
            return BadRequest(new
            {
                estado = 400,
                mensaje = "Solicitud inválida.",
                detalle = e.Message
            });
        }
    }

    [HttpGet("{termino}")]
    public async Task<IActionResult> ObtenerPorTermino(string termino)
    {
        try
        {
            return Ok(await _servicio.ObtenerPorTermino(termino));
        }
        catch (NoEncontradoExcepcion e)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Término clave no encontrado.",
                detalle = e.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Crear(TerminoClaveCrear peticion)
    {
        try
        {
            var entidad = new TerminoClave
            {
                Termino = peticion.Termino,
                TerminoIngles = peticion.TerminoIngles
            };

            await _servicio.Crear(entidad);

            return Ok(new
            {
                estado = 200,
                mensaje = "Término clave creado exitosamente."
            });
        }
        catch (SqlException e)
        {
            return StatusCode(500, new
            {
                estado = 500,
                mensaje = "No se pudo crear el término clave.",
                detalle = e.Message
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                estado = 500,
                mensaje = "Error interno.",
                detalle = e.Message
            });
        }
    }

    [HttpPut("{termino}")]
    public async Task<IActionResult> Reemplazar(
        string termino,
        TerminoClaveReemplazo peticion)
    {
        try
        {
            var entidad = new TerminoClave
            {
                Termino = termino,
                TerminoIngles = peticion.TerminoIngles
            };

            var filas = await _servicio.Reemplazar(termino, entidad);

            return Ok(new
            {
                estado = 200,
                mensaje = "Término clave reemplazado.",
                filasAfectadas = filas
            });
        }
        catch (NoEncontradoExcepcion e)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Término clave no encontrado.",
                detalle = e.Message
            });
        }
    }

    [HttpPatch("{termino}")]
    public async Task<IActionResult> Actualizar(
        string termino,
        TerminoClaveActualizar peticion)
    {
        try
        {
            var campos = new TerminoClaveCampos(
                peticion.TerminoIngles);

            var filas = await _servicio.ActualizarParcial(
                termino,
                campos);

            return Ok(new
            {
                estado = 200,
                mensaje = "Término clave actualizado.",
                filasAfectadas = filas
            });
        }
        catch (ArgumentException e)
        {
            return BadRequest(new
            {
                estado = 400,
                mensaje = "Solicitud inválida.",
                detalle = e.Message
            });
        }
        catch (NoEncontradoExcepcion e)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Término clave no encontrado.",
                detalle = e.Message
            });
        }
    }

    [HttpDelete("{termino}")]
    public async Task<IActionResult> Eliminar(string termino)
    {
        try
        {
            var filas = await _servicio.Eliminar(termino);

            return Ok(new
            {
                estado = 200,
                mensaje = "Término clave eliminado.",
                filasAfectadas = filas
            });
        }
        catch (NoEncontradoExcepcion e)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Término clave no encontrado.",
                detalle = e.Message
            });
        }
    }
}
