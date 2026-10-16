# Contratos — delta V2

Se conservan las rutas y sobres de v1. Cada recurso ofrece GET listado, GET por clave, POST, PUT, PATCH y DELETE lógico. `GET /swagger/v1/swagger.json` publica los cuerpos concretos y sus tipos.

| Tabla | Ruta `/api/…` | Clave de la ruta |
|---|---|---|
| docente | docente | cedula |
| estudios_realizados | estudios-realizados | id |
| docente_departamento | docente-departamento | docente/departamento |
| intereses_futuros | intereses-futuros | docente/terminoClave |
| evaluacion_docente | evaluacion-docente | id |
| reconocimiento | reconocimiento | id |
| experiecia | experiecia | id |
| red_docente | red-docente | red/docente |
| estudio_ac | estudio-ac | estudio/areaConocimiento |
| apoyo_profesoral | apoyo-profesoral | estudios |
| beca | beca | estudios |

Resultados: 200 y datos en operaciones exitosas; listados vacíos conservan la convención anterior de cada recurso (200 con datos vacíos o 204). 400 para límite no positivo o actualización parcial vacía; 404 para inexistente/retirado; 422 para campos inválidos; 409 para relación inexistente/inactiva, clave duplicada o maestro con hijos activos. Los conflictos devuelven `{estado:409,mensaje:"…"}` en español. Los POST incorporan `datos` con la fila persistida, incluidos identificadores generados, sin quitar los campos anteriores.

## Expedientes

- `GET /api/docente/{cedula}/expediente`: `{datos:{maestro,detalles:{estudios,evaluaciones,experiencias,reconocimientos,departamentos,intereses,redes}}}`.
- `POST /api/docente/expediente`: `{docente:<DocenteCrear>,detalles:{…}}` crea maestro y todos sus renglones.
- `POST /api/docente/{cedula}/detalles`: mismo cuerpo, agrega renglones al maestro activo existente; actualiza su ficha dentro de la misma transacción.
- `GET /api/estudios-realizados/{id}/expediente`: `{datos:{maestro,detalles:{areas,apoyos,becas}}}`.
- `POST /api/estudios-realizados/expediente`: `{estudio:<EstudioRealizadoCrear>,detalles:{areas:[],apoyos:[],becas:[]}}`.
- `POST /api/estudios-realizados/{id}/detalles`: actualiza la ficha y agrega renglones al estudio activo existente.

Las colecciones omitidas son vacías. Cada renglón reutiliza el cuerpo Crear de su recurso. La interfaz completa el padre desde el maestro; el repositorio lo fija desde el maestro de la transacción. No se permite elegir otro padre dentro del detalle. Un único envío usa una sola conexión y transacción serializable; un conflicto devuelve 409 y revierte todos los cambios del envío, incluido el trigger. Los identificadores identity no se consideran consecutivos: SQL Server puede consumirlos al revertir.

«Guardar expediente completo» guarda la ficha y los renglones pendientes en una transacción, tanto al crear como al editar. «Guardar la ficha completa» y «Guardar solo lo que cambié», disponibles al editar, permiten conservar las operaciones individuales de la ficha. Un maestro nuevo se crea únicamente con el botón del expediente, incluso si no lleva detalles.
