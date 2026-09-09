using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioTerminoClave
{
    Task<IEnumerable<TerminoClave>> ObtenerTodos(int limite);
    Task<TerminoClave?> ObtenerPorTermino(string termino);
    Task Crear(TerminoClave terminoClave);
    Task<int> Reemplazar(TerminoClave terminoClave);
    Task<int> ActualizarParcial(string termino, TerminoClaveCampos campos);
    Task<int> EliminarLogico(string termino);
}

public record TerminoClaveCampos(string? TerminoIngles)
{
    public bool HayAlguno => TerminoIngles != null;
}
