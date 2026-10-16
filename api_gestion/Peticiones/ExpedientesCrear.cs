using System.ComponentModel.DataAnnotations;
namespace ApiGestion.Peticiones;

public class DetallesDocenteCrear
{
    [Required] public List<EstudioRealizadoCrear> Estudios { get; set; } = new();
    [Required] public List<EvaluacionDocenteCrear> Evaluaciones { get; set; } = new();
    [Required] public List<ExperieciaCrear> Experiencias { get; set; } = new();
    [Required] public List<ReconocimientoCrear> Reconocimientos { get; set; } = new();
    [Required] public List<DocenteDepartamentoCrear> Departamentos { get; set; } = new();
    [Required] public List<InteresFuturoCrear> Intereses { get; set; } = new();
    [Required] public List<RedDocenteCrear> Redes { get; set; } = new();
}

public class DetallesEstudioCrear
{
    [Required] public List<EstudioAcCrear> Areas { get; set; } = new();
    [Required] public List<ApoyoProfesoralCrear> Apoyos { get; set; } = new();
    [Required] public List<BecaCrear> Becas { get; set; } = new();
}

public class ExpedienteDocenteCrear
{
    [Required] public DocenteCrear Docente { get; set; } = new();
    [Required] public DetallesDocenteCrear Detalles { get; set; } = new();
}
public class ExpedienteEstudioCrear
{
    [Required] public EstudioRealizadoCrear Estudio { get; set; } = new();
    [Required] public DetallesEstudioCrear Detalles { get; set; } = new();
}
