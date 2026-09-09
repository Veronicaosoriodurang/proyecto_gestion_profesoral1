using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/linea-investigacion")]
public class LineaInvestigacionController : ControllerBase
{
    private readonly IServicioLineaInvestigacion _servicio;

    public LineaInvestigacionController(IServicioLineaInvestigacion servicio)
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
                tabla = "linea_investigacion",
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
                mensaje = "Línea de investigación no encontrada.",
                detalle = e.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Crear(LineaInvestigacionCrear peticion)
    {
        try
        {
            var linea = new LineaInvestigacion
            {
                Nombre = peticion.Nombre,
                Descripcion = peticion.Descripcion
            };

            await _servicio.Crear(linea);

            return Ok(new
            {
                estado = 200,
                mensaje = "Línea de investigación creada exitosamente."
            });
        }
        catch (SqlException e)
        {
            return StatusCode(500, new
            {
                estado = 500,
                mensaje = "No se pudo crear la línea de investigación.",
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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Reemplazar(
        int id,
        LineaInvestigacionReemplazo peticion)
    {
        try
        {
            var linea = new LineaInvestigacion
            {
                Id = id,
                Nombre = peticion.Nombre,
                Descripcion = peticion.Descripcion
            };

            var filas = await _servicio.Reemplazar(id, linea);

            return Ok(new
            {
                estado = 200,
                mensaje = "Línea de investigación reemplazada.",
                filasAfectadas = filas
            });
        }
        catch (NoEncontradoExcepcion e)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Línea de investigación no encontrada.",
                detalle = e.Message
            });
        }
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        LineaInvestigacionActualizar peticion)
    {
        try
        {
            var campos = new LineaInvestigacionCampos(
                peticion.Nombre,
                peticion.Descripcion);

            var filas = await _servicio.ActualizarParcial(id, campos);

            return Ok(new
            {
                estado = 200,
                mensaje = "Línea de investigación actualizada.",
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
                mensaje = "Línea de investigación no encontrada.",
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
                mensaje = "Línea de investigación eliminada.",
                filasAfectadas = filas
            });
        }
        catch (NoEncontradoExcepcion e)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Línea de investigación no encontrada.",
                detalle = e.Message
            });
        }
    }
}

