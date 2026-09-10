# Gestión Profesoral

![Version](https://img.shields.io/badge/VERSION-v2-2ea44f?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge)
![Blazor](https://img.shields.io/badge/BLAZOR-SERVER-512BD4?style=for-the-badge)
![SQL Server](https://img.shields.io/badge/SQL%20SERVER-2022-CC2927?style=for-the-badge)
![Docker](https://img.shields.io/badge/DOCKER-COMPOSE-2496ED?style=for-the-badge)
![API](https://img.shields.io/badge/API-REST-0078D4?style=for-the-badge)
![Status](https://img.shields.io/badge/STATUS-V2%20COMPLETADA-2ea44f?style=for-the-badge)

Proyecto académico del módulo **Gestión Profesoral**, desarrollado con arquitectura por capas, API REST en .NET, Dapper, SQL Server, Blazor Server y Docker Compose.

La versión **v2** implementa los recursos de la primera versión y todas las relaciones definidas para la segunda versión, con validaciones, manejo de claves foráneas y eliminación lógica.

---

## Estado del proyecto

**Versión actual:** v2 completada

**Recursos v1:**

- Programa
- Red
- Área de conocimiento
- Término clave
- Línea de investigación

**Recursos v2:**

- Docente
- Estudios realizados
- Docente - Departamento
- Intereses futuros
- Evaluación docente
- Reconocimiento
- Experiecia
- Red - Docente
- Estudio - Área de conocimiento
- Apoyo profesoral
- Beca

Todos los recursos de v2 cuentan con API y pantalla Blazor según corresponda, y las operaciones usan borrado lógico mediante el campo `activo`.

---

## Tecnologías

| Tecnología | Uso |
|---|---|
| .NET 10 | API y frontend |
| ASP.NET Core | API REST |
| Blazor Server | Interfaz web |
| Dapper | Acceso a datos |
| SQL Server 2022 | Base de datos |
| Docker Compose | Ejecución del sistema |
| Swagger | Documentación y pruebas de la API |
| Git y GitHub | Control de versiones |

---

## Arquitectura

La API sigue una arquitectura de tres capas:

```text
Controller
    |
    v
Servicio
    |
    v
Repositorio
    |
    v
SQL Server
```

Cada recurso mantiene responsabilidades separadas:

```text
api_gestion/
├── Controllers/
├── Modelos/
├── Peticiones/
├── Repositorios/
├── Servicios/
└── Excepciones/

front_blazor/
├── Components/Pages/
├── Servicios/
└── wwwroot/
```

El frontend consume la API mediante HTTP; no accede directamente a SQL Server.

---

## Ejecución

Solo se necesita **Docker Desktop**.

```powershell
git clone https://github.com/Veronicaosoriodurang/proyecto_gestion_profesoral1.git
cd proyecto_gestion_profesoral1
docker compose up -d --build
```

Servicios principales:

| Servicio | Dirección |
|---|---|
| Aplicación Blazor | http://localhost:8075 |
| API | http://localhost:8074 |
| Swagger | http://localhost:8074/swagger |
| SQL Server | localhost,11472 |

Para detener el proyecto:

```powershell
docker compose down
```

Para reiniciar también los datos de la base:

```powershell
docker compose down -v
```

---

## Endpoints principales

La API expone rutas independientes por recurso. Algunos ejemplos:

```text
/api/programa
/api/red
/api/area-conocimiento
/api/termino-clave
/api/linea-investigacion
/api/docente
/api/estudios-realizados
/api/docente-departamento
/api/intereses-futuros
/api/evaluacion-docente
/api/reconocimiento
/api/experiecia
/api/red-docente
/api/estudio-ac
/api/apoyo-profesoral
/api/beca
```

Las operaciones implementadas incluyen:

```text
GET
POST
PUT
PATCH
DELETE
```

El `DELETE` es lógico: el registro permanece en la base de datos y cambia su estado mediante `activo`.

---

## Validaciones

Entre las validaciones implementadas se encuentran:

- cuerpos incompletos en `PUT` con respuesta 422;
- `PATCH` vacío rechazado;
- límites inválidos rechazados;
- recursos inexistentes o inactivos con respuesta 404;
- validación de claves foráneas antes de crear o modificar relaciones;
- segundo intento de eliminación lógica sobre un registro ya inactivo con respuesta 404;
- validación de rangos y fechas cuando aplica.

---

## Base de datos

La base de datos del módulo contiene las tablas definidas por el proyecto de Gestión Profesoral. La v2 trabaja con las tablas funcionales del módulo y sus relaciones, manteniendo las claves foráneas establecidas en el script SQL original.

El archivo principal se encuentra en:

```text
db/gestion_profesoral.sql
```

---

## Spec Kit

La documentación de especificación se encuentra en `docs/spec_kit/`.

Documentos principales:

| Documento | Contenido |
|---|---|
| `1_constitution.md` | Reglas permanentes del proyecto |
| `0_mapa_versiones.md` | Ruta de versiones v1 a v4 |
| `v1_programa/` | Especificación y planificación de v1 |
| `v2_relaciones/2_spec.md` | Especificación de v2 |
| `v2_relaciones/3_plan.md` | Plan de implementación de v2 |

La especificación define qué debe construirse y el código se desarrolla respetando esas reglas.

---

## Pruebas realizadas en v2

Durante el desarrollo se verificaron los CRUD completos de los recursos de v2, incluyendo casos válidos e inválidos.

Al cierre de la versión se realizó una prueba de regresión sobre los endpoints principales de v1 y v2. Los recursos con datos respondieron `200` y los listados activos sin registros respondieron `204`, sin errores `400`, `404` o `500` en la prueba de lectura final.

También se verificó la compilación de la API y del frontend en .NET 10 dentro de Docker.

---

## Versiones

### v1

CRUD de las tablas sin claves foráneas y estructura inicial del proyecto.

### v2

CRUD y relaciones del módulo Gestión Profesoral, validación de claves foráneas, borrado lógico y pantallas Blazor para los recursos implementados.

### Próximas versiones

El mapa de versiones del Spec Kit define las siguientes etapas del proyecto, incluyendo autenticación y funcionalidades posteriores.

---

## Proyecto académico

Este repositorio corresponde a un proyecto de aula. Su objetivo es aplicar conceptos de servicios web, arquitectura por capas, APIs REST, persistencia de datos, validaciones, Docker, control de versiones y desarrollo frontend con Blazor.
