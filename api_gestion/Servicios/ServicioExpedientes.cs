using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;
namespace ApiGestion.Servicios;

public interface IServicioExpedientes
{
    Task<ExpedienteDocenteVista> ObtenerDocente(int cedula);
    Task<ExpedienteEstudioVista> ObtenerEstudio(int id);
    Task<Docente> GuardarDocente(int? cedula, ExpedienteDocenteCrear peticion);
    Task<EstudioRealizado> GuardarEstudio(int? id, ExpedienteEstudioCrear peticion);
}
public class ServicioExpedientes(IRepositorioExpedientes repositorio) : IServicioExpedientes
{
    public Task<ExpedienteDocenteVista> ObtenerDocente(int cedula) => repositorio.ObtenerDocente(cedula);
    public Task<ExpedienteEstudioVista> ObtenerEstudio(int id) => repositorio.ObtenerEstudio(id);
    public Task<Docente> GuardarDocente(int? cedula, ExpedienteDocenteCrear p)
    {
        if (p.Detalles.Estudios.Any(x => x.InsAcreditada > 1)) throw new ArgumentException("La acreditación debe ser sí o no.");
        if (p.Detalles.Experiencias.Any(x => x.FechaFin < x.FechaInicio) || p.Detalles.Departamentos.Any(x => x.FechaSalida < x.FechaIngreso))
            throw new ArgumentException("La fecha final no puede ser anterior a la inicial.");
        return repositorio.GuardarDocente(cedula, p);
    }
    public Task<EstudioRealizado> GuardarEstudio(int? id, ExpedienteEstudioCrear p)
    {
        if (p.Estudio.InsAcreditada > 1 || p.Detalles.Apoyos.Any(x => x.ConApoyo > 1)) throw new ArgumentException("La selección debe ser sí o no.");
        if (p.Detalles.Becas.Any(x => x.FechaFin < x.FechaInicio)) throw new ArgumentException("La fecha final no puede ser anterior a la inicial.");
        return repositorio.GuardarEstudio(id, p);
    }
}
