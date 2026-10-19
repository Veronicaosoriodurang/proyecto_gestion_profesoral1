namespace ApiGestion.Repositorios;

public interface IReglasRetiro
{
    Task Comprobar(string recurso, object claves);
}
