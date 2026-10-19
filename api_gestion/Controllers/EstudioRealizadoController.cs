using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/estudios-realizados")]
public class EstudioRealizadoController : ControllerBase
{
    private readonly IServicioEstudioRealizado _servicio;

    public EstudioRealizadoController(IServicioEstudioRealizado servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos([FromQuery] int limite = 1000)
    {
        try
        {
            var datos = await _servicio.ObtenerTodos(limite);
            var lista = datos.ToList();

            if (lista.Count == 0)
                return NoContent();

            return Ok(new
            {
                tabla = "estudios_realizados",
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
                mensaje = "Estudio realizado no encontrado.",
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
    public async Task<IActionResult> Crear([FromBody] EstudioRealizadoCrear peticion)
    {
        try
        {
            var estudio = new EstudioRealizado
            {
                Id = peticion.Id!.Value,
                Titulo = peticion.Titulo,
                Universidad = peticion.Universidad,
                Fecha = peticion.Fecha!.Value,
                Tipo = peticion.Tipo,
                Ciudad = peticion.Ciudad,
                Docente = peticion.Docente!.Value,
                InsAcreditada = peticion.InsAcreditada!.Value,
                Metodologia = peticion.Metodologia,
                PerfilEgresado = peticion.PerfilEgresado,
                Pais = peticion.Pais
            };

            await _servicio.Crear(estudio);

            return Ok(new
            {
                datos = estudio, estado = 200,
                mensaje = "Estudio realizado creado exitosamente."
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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Reemplazar(
        int id,
        [FromBody] EstudioRealizadoReemplazo peticion)
    {
        try
        {
            var estudio = new EstudioRealizado
            {
                Id = id,
                Titulo = peticion.Titulo,
                Universidad = peticion.Universidad,
                Fecha = peticion.Fecha!.Value,
                Tipo = peticion.Tipo,
                Ciudad = peticion.Ciudad,
                Docente = peticion.Docente!.Value,
                InsAcreditada = peticion.InsAcreditada!.Value,
                Metodologia = peticion.Metodologia,
                PerfilEgresado = peticion.PerfilEgresado,
                Pais = peticion.Pais
            };

            var filas = await _servicio.Reemplazar(id, estudio);

            return Ok(new
            {
                estado = 200,
                mensaje = "Estudio realizado reemplazado.",
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
                mensaje = "Estudio realizado no encontrado.",
                detalle = ex.Message
            });
        }
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> ActualizarParcial(
        int id,
        [FromBody] EstudioRealizadoActualizar peticion)
    {
        try
        {
            var campos = new EstudioRealizadoCampos(
                peticion.Titulo,
                peticion.Universidad,
                peticion.Fecha,
                peticion.Tipo,
                peticion.Ciudad,
                peticion.Docente,
                peticion.InsAcreditada,
                peticion.Metodologia,
                peticion.PerfilEgresado,
                peticion.Pais);

            var filas = await _servicio.ActualizarParcial(id, campos);

            return Ok(new
            {
                estado = 200,
                mensaje = "Estudio realizado actualizado.",
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
                mensaje = "Estudio realizado no encontrado.",
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
                mensaje = "Estudio realizado eliminado.",
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
                mensaje = "Estudio realizado no encontrado.",
                detalle = ex.Message
            });
        }
    }
}
