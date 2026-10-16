using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

/// <summary>
/// Capa 1 HTTP de docente. Traduce excepciones a códigos de estado
/// (patrón de ProgramaController).
/// </summary>
[ApiController]
[Route("api/docente")]
public class DocenteController : ControllerBase
{
    private readonly IServicioDocente _servicio;

    public DocenteController(IServicioDocente servicio)
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
            {
                return NoContent();
            }

            return Ok(new
            {
                tabla = "docente",
                limite = limite,
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
            return BadRequest(new { estado = 400, mensaje = "Parámetros inválidos.", detalle = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno del servidor.", detalle = ex.Message });
        }
    }

    [HttpGet("{cedula:int}")]
    public async Task<IActionResult> ObtenerPorId(int cedula)
    {
        try
        {
            var docente = await _servicio.ObtenerPorId(cedula);
            return Ok(docente);
        }
        catch (SqlException conflicto) when (conflicto.Number is 547 or 2601 or 2627 or 51001 or 51002)
        { return Conflict(new { estado = 409, mensaje = "La relación no existe, está retirada, tiene registros activos o ya está registrada." }); }
        catch (ApiGestion.Excepciones.ConflictoExcepcion conflicto)
        { return Conflict(new { estado = 409, mensaje = conflicto.Message }); }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { estado = 404, mensaje = "Docente no encontrado.", detalle = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno del servidor.", detalle = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] DocenteCrear peticion)
    {
        try
        {
            var docente = new Docente
            {
                Cedula = peticion.Cedula!.Value,
                Nombres = peticion.Nombres,
                Apellidos = peticion.Apellidos,
                Genero = peticion.Genero,
                Cargo = peticion.Cargo,
                FechaNacimiento = peticion.FechaNacimiento!.Value,
                Correo = peticion.Correo,
                Telefono = peticion.Telefono,
                UrlCvlac = peticion.UrlCvlac,
                FechaActualizacion = peticion.FechaActualizacion!.Value,
                Escalafon = peticion.Escalafon,
                Perfil = peticion.Perfil,
                CatMinciencia = peticion.CatMinciencia,
                ConvMinciencia = peticion.ConvMinciencia,
                Nacionalidaad = peticion.Nacionalidaad,
                LineaInvestigacionPrincipal = peticion.LineaInvestigacionPrincipal
            };

            await _servicio.Crear(docente);
            return Ok(new { datos = docente, estado = 200, mensaje = "Docente creado exitosamente." });
        }
        catch (SqlException conflicto) when (conflicto.Number is 547 or 2601 or 2627 or 51001 or 51002)
        { return Conflict(new { estado = 409, mensaje = "La relación no existe, está retirada, tiene registros activos o ya está registrada." }); }
        catch (ApiGestion.Excepciones.ConflictoExcepcion conflicto)
        { return Conflict(new { estado = 409, mensaje = conflicto.Message }); }
        catch (ArgumentException ex)
        {
            return BadRequest(new { estado = 400, mensaje = "Parámetros inválidos.", detalle = ex.Message });
        }
        catch (SqlException ex)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error al insertar en la base de datos.", detalle = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno del servidor.", detalle = ex.Message });
        }
    }

    [HttpPut("{cedula:int}")]
    public async Task<IActionResult> Reemplazar(int cedula, [FromBody] DocenteReemplazo peticion)
    {
        try
        {
            var docente = new Docente
            {
                Cedula = cedula,
                Nombres = peticion.Nombres,
                Apellidos = peticion.Apellidos,
                Genero = peticion.Genero,
                Cargo = peticion.Cargo,
                FechaNacimiento = peticion.FechaNacimiento!.Value,
                Correo = peticion.Correo,
                Telefono = peticion.Telefono,
                UrlCvlac = peticion.UrlCvlac,
                FechaActualizacion = peticion.FechaActualizacion!.Value,
                Escalafon = peticion.Escalafon,
                Perfil = peticion.Perfil,
                CatMinciencia = peticion.CatMinciencia,
                ConvMinciencia = peticion.ConvMinciencia,
                Nacionalidaad = peticion.Nacionalidaad,
                LineaInvestigacionPrincipal = peticion.LineaInvestigacionPrincipal
            };

            var filas = await _servicio.Reemplazar(cedula, docente);
            return Ok(new { estado = 200, mensaje = "Docente reemplazado.", filasAfectadas = filas });
        }
        catch (SqlException conflicto) when (conflicto.Number is 547 or 2601 or 2627 or 51001 or 51002)
        { return Conflict(new { estado = 409, mensaje = "La relación no existe, está retirada, tiene registros activos o ya está registrada." }); }
        catch (ApiGestion.Excepciones.ConflictoExcepcion conflicto)
        { return Conflict(new { estado = 409, mensaje = conflicto.Message }); }
        catch (ArgumentException ex)
        {
            return BadRequest(new { estado = 400, mensaje = "Parámetros inválidos.", detalle = ex.Message });
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { estado = 404, mensaje = "Docente no encontrado.", detalle = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno del servidor.", detalle = ex.Message });
        }
    }

    [HttpPatch("{cedula:int}")]
    public async Task<IActionResult> ActualizarParcial(int cedula, [FromBody] DocenteActualizar peticion)
    {
        try
        {
            var campos = new DocenteCampos(
                peticion.Nombres, peticion.Apellidos, peticion.Genero, peticion.Cargo,
                peticion.FechaNacimiento, peticion.Correo, peticion.Telefono,
                peticion.UrlCvlac, peticion.FechaActualizacion, peticion.Escalafon,
                peticion.Perfil, peticion.CatMinciencia, peticion.ConvMinciencia,
                peticion.Nacionalidaad, peticion.LineaInvestigacionPrincipal);

            var filas = await _servicio.ActualizarParcial(cedula, campos);
            return Ok(new { estado = 200, mensaje = "Docente actualizado.", filasAfectadas = filas });
        }
        catch (SqlException conflicto) when (conflicto.Number is 547 or 2601 or 2627 or 51001 or 51002)
        { return Conflict(new { estado = 409, mensaje = "La relación no existe, está retirada, tiene registros activos o ya está registrada." }); }
        catch (ApiGestion.Excepciones.ConflictoExcepcion conflicto)
        { return Conflict(new { estado = 409, mensaje = conflicto.Message }); }
        catch (ArgumentException ex)
        {
            return BadRequest(new { estado = 400, mensaje = "Parámetros inválidos.", detalle = ex.Message });
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { estado = 404, mensaje = "Docente no encontrado.", detalle = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno del servidor.", detalle = ex.Message });
        }
    }

    [HttpDelete("{cedula:int}")]
    public async Task<IActionResult> Eliminar(int cedula)
    {
        try
        {
            var filas = await _servicio.Eliminar(cedula);
            return Ok(new { estado = 200, mensaje = "Docente eliminado.", filasAfectadas = filas });
        }
        catch (SqlException conflicto) when (conflicto.Number is 547 or 2601 or 2627 or 51001 or 51002)
        { return Conflict(new { estado = 409, mensaje = "La relación no existe, está retirada, tiene registros activos o ya está registrada." }); }
        catch (ApiGestion.Excepciones.ConflictoExcepcion conflicto)
        { return Conflict(new { estado = 409, mensaje = conflicto.Message }); }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { estado = 404, mensaje = "Docente no encontrado.", detalle = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno del servidor.", detalle = ex.Message });
        }
    }
}
