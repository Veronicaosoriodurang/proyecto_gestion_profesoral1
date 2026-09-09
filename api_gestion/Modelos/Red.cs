namespace ApiGestion.Modelos;

/// <summary>
/// Entidad de dominio que representa la tabla red.
/// El campo Activo no se expone porque se usa solo para el borrado lógico.
/// </summary>
public class Red
{
    public int Idr { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string Pais { get; set; } = string.Empty;
}