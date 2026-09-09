# Mapa de versiones — Módulo Gestión Profesoral

> **Cada versión entrega su API *y* su pantalla** (Artículo 1.1). No hay una
> versión «de front» al final: la v1 ya trae la suya, en Blazor Server, en su
> propio contenedor y en el puerto **8075**. Una versión no está
> cerrada si la API responde y la pantalla no.


> La ruta completa del proyecto. Cada versión se especifica **solo cuando
> la anterior está cerrada** (commit + tag). Este mapa da la dirección; el
> spec kit de cada versión da el detalle.
>
> La ruta es la que define
> [modulo_gestion_profesoral.md](../../../ProyectosDeAula/docs/modulo_gestion_profesoral.md);
> aquí no se inventa nada, se ordena.

## La ruta

| Versión | Qué agrega (acumulativo) | Estado |
|---|---|---|
| **v1** | CRUD completo de las **tablas sin clave foránea**, con API REST y frontend funcionando | **Terminada ✅** |
| v2 | CRUD de las **11 tablas con clave foránea**: las FK como listas desplegables cargadas desde la API, y validación de integridad referencial | Sin especificar |
| v3 | **JWT**, sesiones y control de acceso por roles; CRUD de `usuario`, `rol` y `rol_usuario` solo para administradores | Sin especificar |
| v4 | **10 consultas multitabla** (4+ tablas cada una), dashboard con gráficos, páginas corporativas, responsive/PWA y **publicación** en un servidor | Sin especificar |

## Qué tabla entra en qué versión

Las 19 tablas de la base, repartidas:

| Versión | Tablas |
|---|---|
| **v1** | `programa` · `area_conocimiento` · `termino_clave` · `linea_investigacion` · `red` |
| v2 | `docente` · `estudios_realizados` · `docente_departamento` · `intereses_futuros` · `evaluacion_docente` · `reconocimiento` · `experiecia` · `red_docente` · `estudio_ac` · `apoyo_profesoral` · `beca` |
| v3 | `rol` · `usuario` · `rol_usuario` |

> **Ojo:** las 19 tablas **existen en la base desde la v1** (Artículo 5 de
> la [constitución](../1_constitution.md)). Lo que reparte esta tabla es
> qué puede **nombrar el código** de cada versión, no qué existe en el
> motor.

## Lo que construye la v1

La v1 implementa el CRUD completo, tanto en la API REST como en el frontend
Blazor, de las cinco tablas sin clave foránea del módulo Gestión Profesoral:

- `programa`
- `area_conocimiento`
- `termino_clave`
- `linea_investigacion`
- `red`

Cada recurso cuenta con controlador, servicio, repositorio, interfaces,
peticiones de creación/reemplazo/actualización, validaciones y borrado lógico.

La versión fue verificada de extremo a extremo, integrada mediante Pull Request
a `main` y cerrada con el tag `v1`.
## Reglas del mapa

1. **No se anticipa nada de una versión futura** (Artículo 1 de la
   constitución): en la v1 no aparece una FK, ni un programa, ni un token.
2. **Una versión cerrada no se reabre**: los ajustes van en la siguiente.
3. **Regresión obligatoria**: al cerrar la vN, los criterios de todas las
   versiones anteriores deben seguir pasando.
4. El repositorio siempre muestra la **versión en curso, funcionando**.
