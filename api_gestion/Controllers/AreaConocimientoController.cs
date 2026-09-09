using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/area-conocimiento")]
public class AreaConocimientoController : ControllerBase
{
    private readonly IServicioAreaConocimiento _servicio;

    public AreaConocimientoController(IServicioAreaConocimiento servicio)
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
                tabla = "area_conocimiento",
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

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(string id)
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
                mensaje = "Área de conocimiento no encontrada.",
                detalle = e.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Crear(AreaConocimientoCrear peticion)
    {
        try
        {
            var area = new AreaConocimiento
            {
                Id = peticion.Id,
                GranArea = peticion.GranArea,
                Area = peticion.Area,
                Disciplina = peticion.Disciplina
            };

            await _servicio.Crear(area);

            return Ok(new
            {
                estado = 200,
                mensaje = "Área de conocimiento creada exitosamente."
            });
        }
        catch (SqlException e)
        {
            return StatusCode(500, new
            {
                estado = 500,
                mensaje = "No se pudo crear el área de conocimiento.",
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Reemplazar(
        string id,
        AreaConocimientoReemplazo peticion)
    {
        try
        {
            var area = new AreaConocimiento
            {
                Id = id,
                GranArea = peticion.GranArea,
                Area = peticion.Area,
                Disciplina = peticion.Disciplina
            };

            var filas = await _servicio.Reemplazar(id, area);

            return Ok(new
            {
                estado = 200,
                mensaje = "Área de conocimiento reemplazada.",
                filasAfectadas = filas
            });
        }
        catch (NoEncontradoExcepcion e)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Área de conocimiento no encontrada.",
                detalle = e.Message
            });
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Actualizar(
        string id,
        AreaConocimientoActualizar peticion)
    {
        try
        {
            var campos = new AreaConocimientoCampos(
                peticion.GranArea,
                peticion.Area,
                peticion.Disciplina);

            var filas = await _servicio.ActualizarParcial(id, campos);

            return Ok(new
            {
                estado = 200,
                mensaje = "Área de conocimiento actualizada.",
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
                mensaje = "Área de conocimiento no encontrada.",
                detalle = e.Message
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(string id)
    {
        try
        {
            var filas = await _servicio.Eliminar(id);

            return Ok(new
            {
                estado = 200,
                mensaje = "Área de conocimiento eliminada.",
                filasAfectadas = filas
            });
        }
        catch (NoEncontradoExcepcion e)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Área de conocimiento no encontrada.",
                detalle = e.Message
            });
        }
    }
}
