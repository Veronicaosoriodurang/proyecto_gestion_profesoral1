using Microsoft.Data.SqlClient;
using ApiGestion.Excepciones;
using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/beca")]
public class BecaController : ControllerBase
{
    private readonly IServicioBeca _servicio;

    public BecaController(IServicioBeca servicio)
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
                tabla = "beca",
                limite,
                total = datos.Count,
                datos
            });
        }
        catch (SqlException conflicto) when (conflicto.Number is 547 or 2601 or 2627 or 51001 or 51002)
        { return Conflict(new { estado = 409, mensaje = "La relación no existe, está retirada, tiene registros activos o ya está registrada." }); }
        catch (ApiGestion.Excepciones.ConflictoExcepcion conflicto)
        { return Conflict(new { estado = 409, mensaje = conflicto.Message }); }
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
        catch (SqlException conflicto) when (conflicto.Number is 547 or 2601 or 2627 or 51001 or 51002)
        { return Conflict(new { estado = 409, mensaje = "La relación no existe, está retirada, tiene registros activos o ya está registrada." }); }
        catch (ApiGestion.Excepciones.ConflictoExcepcion conflicto)
        { return Conflict(new { estado = 409, mensaje = conflicto.Message }); }
        catch (NoEncontradoExcepcion e)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Beca no encontrada.",
                detalle = e.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] BecaCrear peticion)
    {
        try
        {
            var filas =
                await _servicio.Crear(peticion);

            return Ok(new
            {
                datos = peticion, estado = 200,
                mensaje = "Beca creada exitosamente.",
                filasAfectadas = filas
            });
        }
        catch (SqlException conflicto) when (conflicto.Number is 547 or 2601 or 2627 or 51001 or 51002)
        { return Conflict(new { estado = 409, mensaje = "La relación no existe, está retirada, tiene registros activos o ya está registrada." }); }
        catch (ApiGestion.Excepciones.ConflictoExcepcion conflicto)
        { return Conflict(new { estado = 409, mensaje = conflicto.Message }); }
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
        [FromBody] BecaReemplazo peticion)
    {
        try
        {
            var filas =
                await _servicio.Reemplazar(
                    estudios,
                    peticion);

            return Ok(new
            {
                estado = 200,
                mensaje = "Beca reemplazada.",
                filasAfectadas = filas
            });
        }
        catch (SqlException conflicto) when (conflicto.Number is 547 or 2601 or 2627 or 51001 or 51002)
        { return Conflict(new { estado = 409, mensaje = "La relación no existe, está retirada, tiene registros activos o ya está registrada." }); }
        catch (ApiGestion.Excepciones.ConflictoExcepcion conflicto)
        { return Conflict(new { estado = 409, mensaje = conflicto.Message }); }
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
                mensaje = "Beca no encontrada.",
                detalle = e.Message
            });
        }
    }

    [HttpPatch("{estudios:int}")]
    public async Task<IActionResult> Actualizar(
        int estudios,
        [FromBody] BecaActualizar peticion)
    {
        try
        {
            var filas =
                await _servicio.ActualizarParcial(
                    estudios,
                    peticion);

            return Ok(new
            {
                estado = 200,
                mensaje = "Beca actualizada.",
                filasAfectadas = filas
            });
        }
        catch (SqlException conflicto) when (conflicto.Number is 547 or 2601 or 2627 or 51001 or 51002)
        { return Conflict(new { estado = 409, mensaje = "La relación no existe, está retirada, tiene registros activos o ya está registrada." }); }
        catch (ApiGestion.Excepciones.ConflictoExcepcion conflicto)
        { return Conflict(new { estado = 409, mensaje = conflicto.Message }); }
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
                mensaje = "Beca no encontrada.",
                detalle = e.Message
            });
        }
    }

    [HttpDelete("{estudios:int}")]
    public async Task<IActionResult> Eliminar(
        int estudios)
    {
        try
        {
            var filas =
                await _servicio.Eliminar(estudios);

            return Ok(new
            {
                estado = 200,
                mensaje = "Beca eliminada.",
                filasAfectadas = filas
            });
        }
        catch (SqlException conflicto) when (conflicto.Number is 547 or 2601 or 2627 or 51001 or 51002)
        { return Conflict(new { estado = 409, mensaje = "La relación no existe, está retirada, tiene registros activos o ya está registrada." }); }
        catch (ApiGestion.Excepciones.ConflictoExcepcion conflicto)
        { return Conflict(new { estado = 409, mensaje = conflicto.Message }); }
        catch (NoEncontradoExcepcion e)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Beca no encontrada.",
                detalle = e.Message
            });
        }
    }
}
