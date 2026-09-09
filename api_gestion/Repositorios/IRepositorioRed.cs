using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioRed
{
    Task<IEnumerable<Red>> ObtenerTodos(int limite);

    Task<Red?> ObtenerPorId(int idr);

    Task Crear(Red red);

    Task<int> Reemplazar(Red red);

    Task<int> ActualizarParcial(int idr, RedCampos campos);

    Task<int> EliminarLogico(int idr);
}

/// <summary>
/// Representa los campos opcionales que pueden llegar en un PATCH.
/// </summary>
public record RedCampos(
    string? Nombre,
    string? Url,
    string? Pais)
{
    public bool HayAlguno =>
        Nombre != null ||
        Url != null ||
        Pais != null;
}