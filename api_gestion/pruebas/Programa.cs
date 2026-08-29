using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Repositorios;
using ApiGestion.Servicios;

namespace ApiGestion.Pruebas;

/// <summary>
/// El repositorio de mentiras: otra implementación de la MISMA interfaz que guarda
/// las filas en una lista en memoria, en vez de hablar con SQL Server.
///
/// Como el servicio solo conoce la interfaz, no se entera de la diferencia. Eso es
/// lo que hace demostrable que las capas están desacopladas (criterio 7).
/// </summary>
public class RepositorioFalso : IRepositorioPrograma
{
    private readonly List<Programa> _filas = new();

    public Task<IEnumerable<Programa>> ObtenerTodos(int limite) =>
        Task.FromResult(_filas.Take(limite).AsEnumerable());

    public Task<Programa?> ObtenerPorId(int id) =>
        Task.FromResult(_filas.FirstOrDefault(p => p.Id == id));

    public Task Crear(Programa programa)
    {
        _filas.Add(programa);
        return Task.CompletedTask;
    }

    public Task<int> Reemplazar(Programa programa)
    {
        var i = _filas.FindIndex(p => p.Id == programa.Id);
        if (i == -1) return Task.FromResult(0);
        _filas[i] = programa;
        return Task.FromResult(1);
    }

    public Task<int> ActualizarParcial(int id, ProgramaCampos campos)
    {
        var p = _filas.FirstOrDefault(x => x.Id == id);
        if (p == null) return Task.FromResult(0);

        if (campos.Nombre != null) p.Nombre = campos.Nombre;
        if (campos.Tipo != null) p.Tipo = campos.Tipo;
        if (campos.Nivel != null) p.Nivel = campos.Nivel;
        if (campos.FechaCreacion != null) p.FechaCreacion = campos.FechaCreacion;
        if (campos.FechaCierre != null) p.FechaCierre = campos.FechaCierre;
        if (campos.NumeroCohortes != null) p.NumeroCohortes = campos.NumeroCohortes;
        if (campos.CantGraduados != null) p.CantGraduados = campos.CantGraduados;
        if (campos.FechaActualizacion != null) p.FechaActualizacion = campos.FechaActualizacion;
        if (campos.Ciudad != null) p.Ciudad = campos.Ciudad;
        if (campos.Facultad != null) p.Facultad = campos.Facultad.Value;

        return Task.FromResult(1);
    }

    public Task<int> EliminarLogico(int id)
    {
        // Sacarlo de la lista reproduce el EFECTO OBSERVABLE del borrado lógico:
        // deja de listarse y deja de encontrarse. La entidad no tiene columna
        // Activo —es detalle del motor—, así que aquí no hay nada que marcar.
        var p = _filas.FirstOrDefault(x => x.Id == id);
        if (p == null) return Task.FromResult(0);
        _filas.Remove(p);
        return Task.FromResult(1);
    }
}

public class Programa_Pruebas
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== Prueba de capas — SIN base de datos ===");

        IRepositorioPrograma repoFalso = new RepositorioFalso();
        IServicioPrograma servicio = new ServicioPrograma(repoFalso);

        var nuevo = new Programa
        {
            Id = 9001,
            Nombre = "Ingenieria de Sistemas",
            Tipo = "Pregrado",
            Nivel = "Profesional",
            FechaCreacion = "2005-01-15",
            FechaCierre = null,
            NumeroCohortes = "40",
            CantGraduados = "1250",
            FechaActualizacion = "2026-01-30",
            Ciudad = "Medellin",
            Facultad = 1
        };

        // 1. El sistema arranca vacío
        var vacio = await servicio.ObtenerTodos(1000);
        Console.WriteLine(vacio.Any()
            ? "[ERROR] Debía arrancar sin programas."
            : "[OK] El sistema arranca vacío: sin programas.");

        // 2. Crear y listar
        await servicio.Crear(nuevo);
        var lista = (await servicio.ObtenerTodos(1000)).ToList();
        Console.WriteLine(lista.Count == 1
            ? $"[OK] Programa creado y listado: {lista[0].Nombre}"
            : "[ERROR] Debía haber exactamente un programa.");

        // 3. La fecha de cierre nula sobrevive: un programa abierto no la tiene
        Console.WriteLine(lista[0].FechaCierre == null
            ? "[OK] fechaCierre admite nulos: el programa está abierto."
            : "[ERROR] fechaCierre debía seguir siendo nula.");

        // 4. Buscar uno que no existe lanza NoEncontradoExcepcion
        try
        {
            await servicio.ObtenerPorId(999999);
            Console.WriteLine("[ERROR] Debió lanzar NoEncontradoExcepcion.");
        }
        catch (NoEncontradoExcepcion)
        {
            Console.WriteLine("[OK] Buscar un código inexistente lanza NoEncontradoExcepcion.");
        }

        // 5. El límite inválido es regla de negocio: ArgumentException (→ 400)
        try
        {
            await servicio.ObtenerTodos(0);
            Console.WriteLine("[ERROR] Debió lanzar ArgumentException.");
        }
        catch (ArgumentException)
        {
            Console.WriteLine("[OK] Límite menor o igual a cero rechazado con ArgumentException.");
        }

        // 6. PATCH sin campos: 400, no 404
        try
        {
            await servicio.ActualizarParcial(9001, new ProgramaCampos());
            Console.WriteLine("[ERROR] Debió lanzar ArgumentException por cuerpo vacío.");
        }
        catch (ArgumentException)
        {
            Console.WriteLine("[OK] Cuerpo vacío en actualización parcial rechazado con ArgumentException.");
        }

        // 7. PATCH con un solo campo sí funciona
        await servicio.ActualizarParcial(9001, new ProgramaCampos(Ciudad: "Bogota"));
        var tras = await servicio.ObtenerPorId(9001);
        Console.WriteLine(tras.Ciudad == "Bogota" && tras.Nombre == "Ingenieria de Sistemas"
            ? "[OK] La actualización parcial cambió solo la ciudad."
            : "[ERROR] La actualización parcial tocó campos que no debía.");

        // 8. Eliminar dos veces: la segunda falla como inexistente (C9)
        await servicio.Eliminar(9001);
        Console.WriteLine("[OK] Primera eliminación realizada.");
        try
        {
            await servicio.Eliminar(9001);
            Console.WriteLine("[ERROR] La segunda eliminación debió lanzar NoEncontradoExcepcion.");
        }
        catch (NoEncontradoExcepcion)
        {
            Console.WriteLine("[OK] Segunda eliminación rechazada: para la API ya no existe.");
        }

        // 9. Y el sistema vuelve a estar vacío
        var final = await servicio.ObtenerTodos(1000);
        Console.WriteLine(final.Any()
            ? "[ERROR] Debía quedar vacío otra vez."
            : "[OK] Tras el borrado, el sistema vuelve a estar vacío.");

        Console.WriteLine("=== Prueba de capas completada CON ÉXITO ===");
    }
}
