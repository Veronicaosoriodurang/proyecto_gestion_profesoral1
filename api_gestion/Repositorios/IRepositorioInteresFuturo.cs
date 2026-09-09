using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioInteresFuturo
{
    Task<IEnumerable<InteresFuturo>> ObtenerTodos(int limite);

    Task<InteresFuturo?> ObtenerPorId(
        int docente,
        string terminoClave);

    Task Crear(InteresFuturo interes);

    Task<int> Reemplazar(
        int docenteActual,
        string terminoActual,
        InteresFuturo interes);

    Task<int> ActualizarParcial(
        int docenteActual,
        string terminoActual,
        InteresFuturoCampos campos);

    Task<int> EliminarLogico(
        int docente,
        string terminoClave);
}

public record InteresFuturoCampos(
    int? Docente = null,
    string? TerminoClave = null)
{
    public bool HayAlguno =>
        Docente != null || TerminoClave != null;
}
