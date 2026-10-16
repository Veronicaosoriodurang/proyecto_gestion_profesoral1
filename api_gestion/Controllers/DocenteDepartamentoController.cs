using ApiGestion.Excepciones;
using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/docente-departamento")]
public class DocenteDepartamentoController : ControllerBase
{
    private readonly IServicioDocenteDepartamento _servicio;

    public DocenteDepartamentoController(
        IServicioDocenteDepartamento servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(
        [FromQuery] int limite = 1000)
    {
        try
        {
            var datos = await _servicio.ObtenerTodos(limite);
            var lista = datos.ToList();

            if (lista.Count == 0)
                return NoContent();

            return Ok(new
            {
                tabla = "docente_departamento",
                limite,
                total = lista.Count,
                datos = lista
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

    [HttpGet("{docente:int}/{departamento:int}")]
    public async Task<IActionResult> ObtenerPorId(
        int docente,
        int departamento)
    {
        try
        {
            return Ok(
                await _servicio.ObtenerPorId(docente, departamento));
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
                mensaje = "Relación docente-departamento no encontrada.",
                detalle = ex.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] DocenteDepartamentoCrear peticion)
    {
        try
        {
            await _servicio.Crear(peticion);

            return Ok(new
            {
                datos = peticion, estado = 200,
                mensaje = "Relación docente-departamento creada exitosamente."
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
                mensaje = "Error al insertar en la base de datos.",
                detalle = ex.Message
            });
        }
    }

    [HttpPut("{docente:int}/{departamento:int}")]
    public async Task<IActionResult> Reemplazar(
        int docente,
        int departamento,
        [FromBody] DocenteDepartamentoReemplazo peticion)
    {
        try
        {
            var filas = await _servicio.Reemplazar(
                docente,
                departamento,
                peticion);

            return Ok(new
            {
                estado = 200,
                mensaje = "Relación docente-departamento reemplazada.",
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
                mensaje = "Relación docente-departamento no encontrada.",
                detalle = ex.Message
            });
        }
    }

    [HttpPatch("{docente:int}/{departamento:int}")]
    public async Task<IActionResult> ActualizarParcial(
        int docente,
        int departamento,
        [FromBody] DocenteDepartamentoActualizar peticion)
    {
        try
        {
            var filas = await _servicio.ActualizarParcial(
                docente,
                departamento,
                peticion);

            return Ok(new
            {
                estado = 200,
                mensaje = "Relación docente-departamento actualizada.",
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
                mensaje = "Relación docente-departamento no encontrada.",
                detalle = ex.Message
            });
        }
    }

    [HttpDelete("{docente:int}/{departamento:int}")]
    public async Task<IActionResult> Eliminar(
        int docente,
        int departamento)
    {
        try
        {
            var filas = await _servicio.Eliminar(
                docente,
                departamento);

            return Ok(new
            {
                estado = 200,
                mensaje = "Relación docente-departamento eliminada.",
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
                mensaje = "Relación docente-departamento no encontrada.",
                detalle = ex.Message
            });
        }
    }
}
