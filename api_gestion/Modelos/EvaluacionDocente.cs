namespace ApiGestion.Modelos;

public class EvaluacionDocente
{
    public int Id { get; set; }
    public float Calificacion { get; set; }
    public string Semestre { get; set; } = string.Empty;
    public int Docente { get; set; }
}
