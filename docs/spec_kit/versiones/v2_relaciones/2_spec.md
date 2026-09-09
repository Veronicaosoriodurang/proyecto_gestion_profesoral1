# Especificación — Versión 2: relaciones y claves foráneas

> **Versión 2** del módulo Gestión Profesoral.
> La v1 ya está terminada. Esta versión agrega los CRUD de las tablas con
> clave foránea, manteniendo la misma arquitectura de API + frontend.

## 1. Propósito

Construir el CRUD completo de las 11 tablas con relaciones del módulo,
respetando las claves foráneas existentes en SQL Server y mostrando esas
relaciones en el frontend mediante listas desplegables cargadas desde la API.

## 2. Alcance

La v2 incluye:

- `docente`
- `estudios_realizados`
- `docente_departamento`
- `intereses_futuros`
- `evaluacion_docente`
- `reconocimiento`
- `experiecia`
- `red_docente`
- `estudio_ac`
- `apoyo_profesoral`
- `beca`

Cada recurso debe tener:

- modelo
- peticiones de crear, reemplazar y actualizar
- repositorio e interfaz
- servicio e interfaz
- controlador
- CRUD completo
- borrado lógico
- pantalla Blazor
- validaciones
- manejo de claves foráneas

## 3. Relaciones

| Tabla | Clave foránea |
|---|---|
| `docente` | `linea_investigacion_principal` → `linea_investigacion.id` |
| `estudios_realizados` | `docente` → `docente.cedula` |
| `docente_departamento` | `docente` → `docente.cedula`; `departamento` → `programa.id` |
| `intereses_futuros` | `docente` → `docente.cedula`; `termino_clave` → `termino_clave.termino` |
| `evaluacion_docente` | `docente` → `docente.cedula` |
| `reconocimiento` | `docente` → `docente.cedula` |
| `experiecia` | `docente` → `docente.cedula` |
| `red_docente` | `red` → `red.idr`; `docente` → `docente.cedula` |
| `estudio_ac` | `estudio` → `estudios_realizados.id`; `area_conocimiento` → `area_conocimiento.id` |
| `apoyo_profesoral` | `estudios` → `estudios_realizados.id` |
| `beca` | `estudios` → `estudios_realizados.id` |

## 4. Reglas funcionales

1. Todos los listados muestran solo registros activos.
2. `DELETE` realiza borrado lógico mediante `activo = 0`.
3. Un registro inexistente o inactivo responde 404.
4. `limite <= 0` responde 400.
5. Un cuerpo inválido responde 422.
6. Las FK deben apuntar a registros existentes y activos.
7. El frontend no debe pedir al usuario memorizar IDs cuando exista una
   entidad relacionada que pueda mostrarse en una lista desplegable.
8. Las listas desplegables se cargan desde la API.
9. No se implementa JWT, usuarios ni roles en esta versión.
10. No se implementan todavía dashboard ni consultas multitabla de la v4.

## 5. Listas desplegables del frontend

- Docente: línea de investigación principal.
- Estudios realizados: docente.
- Docente departamento: docente y programa.
- Intereses futuros: docente y término clave.
- Evaluación docente: docente.
- Reconocimiento: docente.
- Experiencia: docente.
- Red docente: red y docente.
- Estudio área de conocimiento: estudio realizado y área de conocimiento.
- Apoyo profesoral: estudio realizado.
- Beca: estudio realizado.

## 6. Orden de implementación

Para respetar las dependencias:

1. `docente`
2. `estudios_realizados`
3. `docente_departamento`
4. `intereses_futuros`
5. `evaluacion_docente`
6. `reconocimiento`
7. `experiecia`
8. `red_docente`
9. `estudio_ac`
10. `apoyo_profesoral`
11. `beca`

## 7. Definición de TERMINADA

La v2 estará terminada cuando:

1. Los 11 CRUD funcionen en API y frontend.
2. Todas las FK se seleccionen correctamente desde la interfaz.
3. No sea posible guardar relaciones con registros inexistentes o inactivos.
4. El borrado lógico funcione en las 11 tablas.
5. Las pruebas de regresión de la v1 sigan pasando.
6. API y frontend compilen correctamente.
7. No queden errores pendientes en Swagger ni en las pantallas.
8. La versión se integre a `main` mediante Pull Request.
9. Se cree el tag `v2`.

