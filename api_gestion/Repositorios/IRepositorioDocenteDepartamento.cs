using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

/// <summary>
/// Contrato de la capa de datos para docente_departamento.
/// </summary>
public interface IRepositorioDocenteDepartamento
{
    Task<IEnumerable<DocenteDepartamento>> ObtenerTodos(int limite);
    Task<DocenteDepartamento?> ObtenerPorId(int docente, int departamento);
    Task Crear(DocenteDepartamento relacion);
    Task<int> Reemplazar(DocenteDepartamento relacion);
    Task<int> ActualizarParcial(
        int docente,
        int departamento,
        DocenteDepartamentoCampos campos);
    Task<int> EliminarLogico(int docente, int departamento);
}

/// <summary>
/// Campos opcionales que puede traer un PATCH.
/// </summary>
public record DocenteDepartamentoCampos(
    string? Dedicacion = null,
    string? Modalidad = null,
    DateOnly? FechaIngreso = null,
    DateOnly? FechaSalida = null)
{
    public bool HayAlguno =>
        Dedicacion != null
        || Modalidad != null
        || FechaIngreso != null
        || FechaSalida != null;
}
