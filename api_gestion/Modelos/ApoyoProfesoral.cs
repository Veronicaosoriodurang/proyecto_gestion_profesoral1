namespace ApiGestion.Modelos;

public class ApoyoProfesoral
{
    public int Estudios { get; set; }
    public byte ConApoyo { get; set; }
    public string Institucion { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
}
