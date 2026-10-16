using ApiGestion.Excepciones;
using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/evaluacion-docente")]
public class EvaluacionDocenteController : ControllerBase
{
    private readonly IServicioEvaluacionDocente _servicio;

    public EvaluacionDocenteController(
        IServicioEvaluacionDocente servicio)
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
                (await _servicio.ObtenerTodos(limite)).ToList();

            if (datos.Count == 0)
                return NoContent();

            return Ok(new
            {
                tabla = "evaluacion_docente",
                limite,
                total = datos.Count,
                datos
            });
        }
        catch (SqlException conflicto) when (conflicto.Number is 547 or 2601 or 2627 or 51001 or 51002)
        { return Conflict(new { estado = 409, mensaje = "La relación no existe, está retirada, tiene registros activos o ya está registrada." }); }
        catch (ApiGestion.Excepciones.ConflictoExcepcion conflicto)
        { return Conflict(new { estado = 409, mensaje = conflicto.Message }); }
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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        try
        {
            return Ok(await _servicio.ObtenerPorId(id));
        }
        catch (SqlException conflicto) when (conflicto.Number is 547 or 2601 or 2627 or 51001 or 51002)
        { return Conflict(new { estado = 409, mensaje = "La relación no existe, está retirada, tiene registros activos o ya está registrada." }); }
        catch (ApiGestion.Excepciones.ConflictoExcepcion conflicto)
        { return Conflict(new { estado = 409, mensaje = conflicto.Message }); }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Evaluación no encontrada.",
                detalle = ex.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] EvaluacionDocenteCrear peticion)
    {
        try
        {
            var id = await _servicio.Crear(peticion);

            return Ok(new
            {
                datos = await _servicio.ObtenerPorId(id), estado = 200,
                mensaje = "Evaluación docente creada exitosamente.",
                id
            });
        }
        catch (SqlException conflicto) when (conflicto.Number is 547 or 2601 or 2627 or 51001 or 51002)
        { return Conflict(new { estado = 409, mensaje = "La relación no existe, está retirada, tiene registros activos o ya está registrada." }); }
        catch (ApiGestion.Excepciones.ConflictoExcepcion conflicto)
        { return Conflict(new { estado = 409, mensaje = conflicto.Message }); }
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
                mensaje = "Error al crear la evaluación.",
                detalle = ex.Message
            });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Reemplazar(
        int id,
        [FromBody] EvaluacionDocenteReemplazo peticion)
    {
        try
        {
            var filas = await _servicio.Reemplazar(
                id,
                peticion);

            return Ok(new
            {
                estado = 200,
                mensaje = "Evaluación docente reemplazada.",
                filasAfectadas = filas
            });
        }
        catch (SqlException conflicto) when (conflicto.Number is 547 or 2601 or 2627 or 51001 or 51002)
        { return Conflict(new { estado = 409, mensaje = "La relación no existe, está retirada, tiene registros activos o ya está registrada." }); }
        catch (ApiGestion.Excepciones.ConflictoExcepcion conflicto)
        { return Conflict(new { estado = 409, mensaje = conflicto.Message }); }
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
                mensaje = "Evaluación no encontrada.",
                detalle = ex.Message
            });
        }
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] EvaluacionDocenteActualizar peticion)
    {
        try
        {
            var filas = await _servicio.ActualizarParcial(
                id,
                peticion);

            return Ok(new
            {
                estado = 200,
                mensaje = "Evaluación docente actualizada.",
                filasAfectadas = filas
            });
        }
        catch (SqlException conflicto) when (conflicto.Number is 547 or 2601 or 2627 or 51001 or 51002)
        { return Conflict(new { estado = 409, mensaje = "La relación no existe, está retirada, tiene registros activos o ya está registrada." }); }
        catch (ApiGestion.Excepciones.ConflictoExcepcion conflicto)
        { return Conflict(new { estado = 409, mensaje = conflicto.Message }); }
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
                mensaje = "Evaluación no encontrada.",
                detalle = ex.Message
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
                mensaje = "Evaluación docente eliminada.",
                filasAfectadas = filas
            });
        }
        catch (SqlException conflicto) when (conflicto.Number is 547 or 2601 or 2627 or 51001 or 51002)
        { return Conflict(new { estado = 409, mensaje = "La relación no existe, está retirada, tiene registros activos o ya está registrada." }); }
        catch (ApiGestion.Excepciones.ConflictoExcepcion conflicto)
        { return Conflict(new { estado = 409, mensaje = conflicto.Message }); }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new
            {
                estado = 404,
                mensaje = "Evaluación no encontrada.",
                detalle = ex.Message
            });
        }
    }
}
