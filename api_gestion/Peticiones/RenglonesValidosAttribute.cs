using System.Collections;
using System.ComponentModel.DataAnnotations;
namespace ApiGestion.Peticiones;

// ModelState debe rechazar un renglón null antes de abrir la transacción.
public sealed class RenglonesValidosAttribute : ValidationAttribute
{
    public RenglonesValidosAttribute() : base("La lista no puede contener renglones vacíos.") { }
    public override bool IsValid(object? value) => value is not IEnumerable filas || filas.Cast<object?>().All(fila => fila != null);
}
