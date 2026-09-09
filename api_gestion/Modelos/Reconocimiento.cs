namespace ApiGestion.Modelos;

public class Reconocimiento
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public DateOnly Fecha { get; set; }
    public string Institucion { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Ambito { get; set; } = string.Empty;
    public int Docente { get; set; }
}
