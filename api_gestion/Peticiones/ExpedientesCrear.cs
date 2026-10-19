using System.ComponentModel.DataAnnotations;
namespace ApiGestion.Peticiones;

public class DetallesDocenteCrear
{
    [Required(ErrorMessage = "La colección no puede ser nula."), RenglonesValidos] public List<EstudioRealizadoCrear> Estudios { get; set; } = new();
    [Required(ErrorMessage = "La colección no puede ser nula."), RenglonesValidos] public List<EvaluacionDocenteCrear> Evaluaciones { get; set; } = new();
    [Required(ErrorMessage = "La colección no puede ser nula."), RenglonesValidos] public List<ExperieciaCrear> Experiencias { get; set; } = new();
    [Required(ErrorMessage = "La colección no puede ser nula."), RenglonesValidos] public List<ReconocimientoCrear> Reconocimientos { get; set; } = new();
    [Required(ErrorMessage = "La colección no puede ser nula."), RenglonesValidos] public List<DocenteDepartamentoCrear> Departamentos { get; set; } = new();
    [Required(ErrorMessage = "La colección no puede ser nula."), RenglonesValidos] public List<InteresFuturoCrear> Intereses { get; set; } = new();
    [Required(ErrorMessage = "La colección no puede ser nula."), RenglonesValidos] public List<RedDocenteCrear> Redes { get; set; } = new();
}

public class DetallesEstudioCrear
{
    [Required(ErrorMessage = "La colección no puede ser nula."), RenglonesValidos] public List<EstudioAcCrear> Areas { get; set; } = new();
    [Required(ErrorMessage = "La colección no puede ser nula."), RenglonesValidos] public List<ApoyoProfesoralCrear> Apoyos { get; set; } = new();
    [Required(ErrorMessage = "La colección no puede ser nula."), RenglonesValidos] public List<BecaCrear> Becas { get; set; } = new();
}

public class ExpedienteDocenteCrear
{
    [Required(ErrorMessage = "La ficha del docente es obligatoria.")] public DocenteCrear Docente { get; set; } = new();
    [Required] public DetallesDocenteCrear Detalles { get; set; } = new();
}
public class ExpedienteEstudioCrear
{
    [Required(ErrorMessage = "La ficha del estudio es obligatoria.")] public EstudioRealizadoCrear Estudio { get; set; } = new();
    [Required] public DetallesEstudioCrear Detalles { get; set; } = new();
}
