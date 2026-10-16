# Modelo de datos — delta V2

Se mantienen las 19 tablas dadas. Las cinco tablas v1 y las once tablas v2 están enumeradas en `../0_mapa_versiones.md`. El manifiesto `db/v2_recursos.json` documenta tipos, claves y relaciones usados en las pruebas; el SQL versionado es la definición ejecutable.

Única columna nueva: `docente.total_estudios INT NOT NULL DEFAULT 0`. Es calculada por `trg_contar_estudios` y no se acepta en las peticiones de escritura. La migración recalcula los contadores previos, sin borrar filas ni cambiar `activo`.

Todos los recursos conservan `activo`: consulta/listado ocultan retirados; eliminar los retira. Claves compuestas: docente_departamento(docente,departamento), intereses_futuros(docente,termino_clave), red_docente(red,docente), estudio_ac(estudio,area_conocimiento). Repetir claves retiradas también da 409. No se reactivan parejas como efecto secundario de crear.

Relación opcional: docente.linea_investigacion_principal admite null. Campos opcionales adicionales: cat_minciencia, fecha_cierre, termino_ingles, fecha_salida y fechas finales según el esquema. PUT puede limpiarlos; PATCH conserva null como omisión por compatibilidad con v1.

Docente contiene estudios, evaluaciones, experiencias, reconocimientos, departamentos, intereses y redes. Estudio contiene áreas, apoyo y beca. Las relaciones históricas `departamento → programa` y los nombres `experiecia`, `nacionalidaad` se conservan tal como están dados.
