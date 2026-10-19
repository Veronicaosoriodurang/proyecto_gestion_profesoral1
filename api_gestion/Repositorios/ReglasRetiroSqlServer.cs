using System.Data;
using ApiGestion.Excepciones;
using Dapper;
using Microsoft.Data.SqlClient;
namespace ApiGestion.Repositorios;

// Lista cerrada: no se acepta un nombre de tabla desde HTTP.
public class ReglasRetiroSqlServer(IConfiguration configuracion) : IReglasRetiro
{
    public async Task Comprobar(string recurso, object claves)
    {
        var procedimiento = recurso switch
        {
            "area_conocimiento" => "dbo.sp_consultar_retiro_area_conocimiento",
            "termino_clave" => "dbo.sp_consultar_retiro_termino_clave",
            "linea_investigacion" => "dbo.sp_consultar_retiro_linea_investigacion",
            "programa" => "dbo.sp_consultar_retiro_programa",
            "red" => "dbo.sp_consultar_retiro_red",
            "docente" => "dbo.sp_consultar_retiro_docente",
            "estudios_realizados" => "dbo.sp_consultar_retiro_estudios_realizados",
            _ => throw new ArgumentException("Recurso de retiro desconocido.")
        };
        await using var conexion = new SqlConnection(configuracion.GetConnectionString("SqlServer"));
        if (await conexion.QuerySingleAsync<bool>(procedimiento, claves, commandType: CommandType.StoredProcedure))
            throw new ConflictoExcepcion("Retire primero los registros relacionados activos.");
    }
}
