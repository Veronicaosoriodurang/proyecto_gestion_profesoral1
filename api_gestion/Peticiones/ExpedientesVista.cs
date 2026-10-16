using ApiGestion.Modelos;
namespace ApiGestion.Peticiones;

public class DetallesDocenteVista
{
    public List<EstudioRealizado> Estudios { get; set; } = new();
    public List<EvaluacionDocente> Evaluaciones { get; set; } = new();
    public List<Experiecia> Experiencias { get; set; } = new();
    public List<Reconocimiento> Reconocimientos { get; set; } = new();
    public List<DocenteDepartamento> Departamentos { get; set; } = new();
    public List<InteresFuturo> Intereses { get; set; } = new();
    public List<RedDocente> Redes { get; set; } = new();
}
public class ExpedienteDocenteVista
{
    public Docente Maestro { get; set; } = new();
    public DetallesDocenteVista Detalles { get; set; } = new();
}
public class DetallesEstudioVista
{
    public List<EstudioAc> Areas { get; set; } = new();
    public List<ApoyoProfesoral> Apoyos { get; set; } = new();
    public List<Beca> Becas { get; set; } = new();
}
public class ExpedienteEstudioVista
{
    public EstudioRealizado Maestro { get; set; } = new();
    public DetallesEstudioVista Detalles { get; set; } = new();
}
