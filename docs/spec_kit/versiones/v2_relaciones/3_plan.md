# Plan — Versión 2: relaciones y claves foráneas

## Objetivo

Implementar la v2 sin modificar el comportamiento ya terminado de la v1.

La arquitectura debe conservar exactamente el patrón existente:

Controller
→ IServicio
→ Servicio
→ IRepositorio
→ RepositorioSqlServer
→ SQL Server

El frontend Blazor consume únicamente la API por HTTP.

## Reglas para implementar

1. No crear servicios, controladores ni repositorios genéricos.
2. Cada tabla tendrá sus propias clases.
3. Mantener SQL escrito a mano y parametrizado con Dapper.
4. Mantener borrado lógico con `activo = 0`.
5. Los GET solo muestran registros activos.
6. Las claves foráneas deben validarse antes de guardar.
7. Una FK debe apuntar a un registro existente y activo.
8. En el frontend las FK se seleccionan mediante listas desplegables.
9. Los desplegables obtienen sus opciones desde la API.
10. No modificar los CRUD de v1 salvo que sea estrictamente necesario para consultar sus catálogos.
11. No implementar JWT, usuarios, roles, dashboard ni funciones de v3/v4.

## Orden de trabajo

### Fase 1 — Docente

Implementar completamente:

- modelo
- peticiones
- repositorio
- servicio
- controlador
- registro de dependencias
- servicio frontend
- pantalla Blazor
- selector de línea de investigación
- pruebas CRUD y FK

No continuar con la siguiente tabla hasta que docente funcione de extremo a extremo.

### Fase 2 — Estudios realizados

Implementar CRUD y selector de docente.

### Fase 3 — Relaciones directas con docente

- `docente_departamento`
- `intereses_futuros`
- `evaluacion_docente`
- `reconocimiento`
- `experiecia`
- `red_docente`

### Fase 4 — Relaciones con estudios

- `estudio_ac`
- `apoyo_profesoral`
- `beca`

## Validaciones mínimas

Para cada recurso:

- POST inválido → 422
- PUT incompleto → 422
- PATCH vacío → 400
- registro inexistente/inactivo → 404
- `limite <= 0` → 400
- segundo DELETE → 404
- DELETE → borrado lógico
- FK inexistente → error controlado
- FK inactiva → error controlado

## Regresión

Antes de cerrar la v2 deben continuar funcionando:

- `/programas`
- `/redes`
- `/areas-conocimiento`
- `/terminos-clave`
- `/lineas-investigacion`

## Estrategia con Cursor

Cursor debe trabajar una tabla a la vez.

Antes de escribir código debe leer:

- `docs/spec_kit/1_constitution.md`
- `docs/spec_kit/versiones/v2_relaciones/2_spec.md`
- este archivo
- los archivos existentes de `Programa`
- un CRUD de v1 con patrón similar

No debe reemplazar archivos existentes de v1 ni cambiar convenciones sin autorización.

La primera implementación será únicamente `docente`.
