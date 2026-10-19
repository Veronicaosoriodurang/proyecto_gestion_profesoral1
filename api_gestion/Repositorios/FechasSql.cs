using System.Data;
using Dapper;
namespace ApiGestion.Repositorios;

internal static class FechasSql
{
    static FechasSql() { SqlMapper.AddTypeHandler(new Fecha()); }
    internal static void Configurar() { }
    private sealed class Fecha : SqlMapper.TypeHandler<DateOnly>
    {
        public override void SetValue(IDbDataParameter p, DateOnly valor) { p.DbType = DbType.Date; p.Value = valor.ToDateTime(TimeOnly.MinValue); }
        public override DateOnly Parse(object valor) => valor is DateOnly fecha ? fecha : DateOnly.FromDateTime((DateTime)valor);
    }
}
