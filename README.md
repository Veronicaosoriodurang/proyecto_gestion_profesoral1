<div align="center">

# GESTIÓN PROFESORAL

### Sistema académico para organizar, relacionar y administrar la información de los docentes

![Version](https://img.shields.io/badge/VERSION-v2-2ea44f?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge)
![Blazor](https://img.shields.io/badge/BLAZOR-SERVER-512BD4?style=for-the-badge)
![SQL Server](https://img.shields.io/badge/SQL%20SERVER-2022-CC2927?style=for-the-badge)
![Docker](https://img.shields.io/badge/DOCKER-COMPOSE-2496ED?style=for-the-badge)
![API](https://img.shields.io/badge/API-REST-0078D4?style=for-the-badge)
![Estado](https://img.shields.io/badge/ESTADO-V2%20COMPLETADA-2ea44f?style=for-the-badge)

**Proyecto de aula — Aplicación y Servicios Web**

</div>

---

## Contenido

- [De qué trata el proyecto](#de-qué-trata-el-proyecto)
- [Por qué se desarrolla](#por-qué-se-desarrolla)
- [Qué está terminado](#qué-está-terminado)
- [Cómo ejecutar el proyecto](#cómo-ejecutar-el-proyecto)
- [Cómo funciona](#cómo-funciona)
- [Estructura del proyecto](#estructura-del-proyecto)
- [Recursos y rutas](#recursos-y-rutas)
- [Operaciones CRUD](#operaciones-crud)
- [Validaciones](#validaciones)
- [Base de datos](#base-de-datos)
- [Spec Kit](#spec-kit)
- [Pruebas realizadas](#pruebas-realizadas)
- [Material conceptual del curso](#material-conceptual-del-curso)
- [Versiones](#versiones)
- [Información académica](#información-académica)

---

## De qué trata el proyecto

**Gestión Profesoral** es una aplicación web pensada para organizar en un solo sistema la información académica y profesional de los docentes de una institución educativa.

La información de un docente no se limita a su nombre o a la dependencia a la que pertenece. También puede incluir sus estudios realizados, áreas de conocimiento, experiencia, evaluaciones, reconocimientos, participación en redes académicas, intereses futuros, apoyos institucionales y becas.

Cuando estos datos se manejan por separado es más difícil consultarlos, relacionarlos y mantenerlos actualizados. Por eso este proyecto propone una solución donde la información se almacena de manera estructurada y cada dato puede relacionarse con los demás de forma controlada.

La aplicación permite registrar, consultar, actualizar y retirar información desde una interfaz web. El usuario trabaja en **Blazor**, la **API REST** recibe y valida cada solicitud, y **SQL Server** almacena los datos.

La idea central del sistema se puede resumir así:

> **Blazor muestra, la API valida y procesa, y SQL Server guarda.**

---

## Por qué se desarrolla

Este proyecto se realiza como parte de la asignatura **Aplicación y Servicios Web** y tiene dos propósitos principales.

El primero es resolver un caso académico realista: construir un sistema capaz de administrar información profesoral y sus relaciones sin guardar datos aislados o inconsistentes.

El segundo es aplicar en un mismo proyecto los temas trabajados en clase, entre ellos:

- construcción de una API REST;
- operaciones CRUD;
- arquitectura por capas;
- validación de información;
- claves primarias y claves foráneas;
- borrado lógico;
- acceso a datos con Dapper;
- desarrollo de interfaces con Blazor;
- ejecución del sistema con Docker Compose;
- documentación mediante Spec Kit;
- uso de Git y GitHub para control de versiones.

Por esta razón el proyecto no consiste únicamente en crear pantallas. También busca demostrar una organización clara del código, separación de responsabilidades, control de errores y trazabilidad entre lo que se especifica, lo que se programa y lo que finalmente se prueba.

---

## Qué está terminado

La versión actual es **v2**.

### Versión 1

La v1 implementa los recursos sin claves foráneas:

| Recurso | Estado |
|---|---|
| Programa | Completo |
| Red | Completo |
| Área de conocimiento | Completo |
| Término clave | Completo |
| Línea de investigación | Completo |

### Versión 2

La v2 agrega los recursos que dependen de otras tablas mediante claves foráneas:

| Recurso | Relación principal | Estado |
|---|---|---|
| Docente | Línea de investigación | Completo |
| Estudios realizados | Docente | Completo |
| Docente - Departamento | Docente y Programa | Completo |
| Intereses futuros | Docente y Término clave | Completo |
| Evaluación docente | Docente | Completo |
| Reconocimiento | Docente | Completo |
| Experiecia | Docente | Completo |
| Red - Docente | Red y Docente | Completo |
| Estudio - Área de conocimiento | Estudio y Área | Completo |
| Apoyo profesoral | Estudio realizado | Completo |
| Beca | Estudio realizado | Completo |

**Resultado de v2: 11 de 11 recursos completados.**

Cada recurso de la v2 tiene su lógica de API y su pantalla Blazor correspondiente.

---

## Cómo ejecutar el proyecto

### Requisito

Solo se necesita tener **Docker Desktop** instalado y funcionando.

### 1. Clonar el repositorio

```powershell
git clone https://github.com/Veronicaosoriodurang/proyecto_gestion_profesoral1.git
cd proyecto_gestion_profesoral1
```

### 2. Levantar todo el sistema

```powershell
docker compose up -d --build
```

Docker se encarga de levantar SQL Server, la API y el frontend.

### 3. Abrir la aplicación

| Servicio | Dirección |
|---|---|
| Aplicación Blazor | http://localhost:8075 |
| API | http://localhost:8074 |
| Swagger | http://localhost:8074/swagger |
| SQL Server | `localhost,11472` |

### Comandos útiles

Encender nuevamente:

```powershell
docker compose up -d
```

Apagar:

```powershell
docker compose down
```

Apagar y reiniciar los datos de la base:

```powershell
docker compose down -v
```

---

## Cómo funciona

La idea principal se puede resumir así:

```text
Usuario
   |
   v
Blazor Server
   |
   | HTTP
   v
API REST
   |
   v
Servicio
   |
   v
Repositorio con Dapper
   |
   v
SQL Server
```

Dentro de la API se usa una arquitectura por capas.

### Controller

Recibe la petición HTTP y devuelve la respuesta correspondiente.

Ejemplos de respuestas:

```text
200  operación correcta
204  listado sin registros activos
400  solicitud inválida
404  recurso inexistente o inactivo
422  cuerpo obligatorio incompleto
```

### Servicio

Contiene las reglas del negocio.

Por ejemplo, antes de crear una beca se comprueba que el estudio relacionado exista y esté activo.

### Repositorio

Es la capa que ejecuta el SQL mediante **Dapper**.

Esta separación permite que cada parte tenga una responsabilidad clara.

---

## Estructura del proyecto

```text
proyecto_gestion_profesoral1/
|
|-- api_gestion/
|   |-- Controllers/        recibe las peticiones HTTP
|   |-- Modelos/            representa los datos
|   |-- Peticiones/         valida los cuerpos de entrada
|   |-- Servicios/          contiene las reglas del negocio
|   |-- Repositorios/       consulta SQL Server con Dapper
|   `-- Excepciones/        maneja errores controlados
|
|-- front_blazor/
|   |-- Components/Pages/   pantallas de cada recurso
|   |-- Servicios/          comunicación HTTP con la API
|   `-- wwwroot/            archivos y estilos del frontend
|
|-- db/
|   `-- gestion_profesoral.sql
|
|-- docs/
|   `-- spec_kit/           especificaciones y planes del proyecto
|
|-- pruebas_humo/           pruebas generales del sistema
|-- postman/                recursos de prueba de la API
|-- ProyectosDeAula/        material entregado para el curso
`-- docker-compose.yml      definición de los servicios Docker
```

La regla más importante para entender esta estructura es:

> **Blazor muestra, la API decide y SQL Server guarda.**

---

## Recursos y rutas

La API tiene una ruta específica para cada recurso.

| Recurso | Ruta principal |
|---|---|
| Programa | `/api/programa` |
| Red | `/api/red` |
| Área de conocimiento | `/api/area-conocimiento` |
| Término clave | `/api/termino-clave` |
| Línea de investigación | `/api/linea-investigacion` |
| Docente | `/api/docente` |
| Estudios realizados | `/api/estudios-realizados` |
| Docente - Departamento | `/api/docente-departamento` |
| Intereses futuros | `/api/intereses-futuros` |
| Evaluación docente | `/api/evaluacion-docente` |
| Reconocimiento | `/api/reconocimiento` |
| Experiecia | `/api/experiecia` |
| Red - Docente | `/api/red-docente` |
| Estudio - Área | `/api/estudio-ac` |
| Apoyo profesoral | `/api/apoyo-profesoral` |
| Beca | `/api/beca` |

Swagger permite consultar y probar estas rutas desde:

```text
http://localhost:8074/swagger
```

---

## Operaciones CRUD

Los recursos implementan las operaciones necesarias para administrar los datos.

| Método | Función |
|---|---|
| `GET` | Consultar |
| `POST` | Crear |
| `PUT` | Reemplazar un registro completo |
| `PATCH` | Actualizar solo algunos campos |
| `DELETE` | Retirar mediante borrado lógico |

### PUT y PATCH

Una diferencia importante del proyecto es:

- **PUT** espera los datos obligatorios del recurso completo.
- **PATCH** permite enviar únicamente los campos que se quieren modificar.

Por eso un PUT incompleto puede responder `422`, mientras que un PATCH válido puede actualizar un solo campo.

### Borrado lógico

El `DELETE` no elimina físicamente la fila de SQL Server.

El registro queda almacenado y su campo `activo` cambia para indicar que ya no debe aparecer como disponible.

```text
activo = 1   registro activo
activo = 0   registro retirado
```

---

## Validaciones

La aplicación controla errores antes de guardar información incorrecta.

Entre las validaciones implementadas están:

- campos obligatorios;
- cuerpos incompletos en `PUT`;
- rechazo de un `PATCH` vacío;
- límites de consulta inválidos;
- recursos inexistentes;
- recursos inactivos;
- claves foráneas inexistentes o inactivas;
- segundo intento de eliminación de un registro ya retirado;
- rangos permitidos;
- validaciones de fechas cuando corresponde.

Ejemplo: una beca no permite que `fecha_fin` sea anterior a `fecha_inicio`.

---

## Base de datos

El script principal se encuentra en:

```text
db/gestion_profesoral.sql
```

SQL Server mantiene las claves primarias y las claves foráneas definidas para el módulo.

En la v2 las relaciones son especialmente importantes. Por ejemplo:

```text
Docente ---- Estudios realizados ---- Beca
   |
   +-------- Evaluación docente
   |
   +-------- Reconocimiento
   |
   +-------- Experiecia
```

Antes de guardar una relación, el servicio comprueba que el registro relacionado exista y se encuentre activo.

---

## Spec Kit

El proyecto sigue una metodología basada en especificaciones.

La documentación principal está en:

```text
docs/spec_kit/
```

| Documento | Para qué sirve |
|---|---|
| `1_constitution.md` | Reglas permanentes del proyecto |
| `0_mapa_versiones.md` | Define qué se construye en cada versión |
| `v1_programa/2_spec.md` | Especificación de la v1 |
| `v1_programa/3_plan.md` | Plan de construcción de la v1 |
| `v2_relaciones/2_spec.md` | Qué debe cumplir la v2 |
| `v2_relaciones/3_plan.md` | Orden y forma de construir la v2 |

La idea del Spec Kit es sencilla:

> **Primero se define qué debe hacer el sistema y después se programa.**

---

## Pruebas realizadas

Durante la v2 cada recurso fue probado individualmente.

Se verificaron casos como:

```text
POST válido                 -> 200
GET válido                  -> 200
PUT válido                  -> 200
PATCH válido                -> 200
PATCH vacío                 -> 400
PUT incompleto              -> 422
recurso inexistente         -> 404
DELETE válido               -> 200
segundo DELETE              -> 404
```

También se probaron las claves foráneas y las reglas particulares de cada tabla.

Al finalizar la v2 se ejecutó una prueba de regresión sobre los recursos de v1 y v2:

- los recursos con datos respondieron `200`;
- los listados activos sin registros respondieron `204`;
- no aparecieron errores `400`, `404` ni `500` en la prueba final de lectura.

La API y el frontend también fueron compilados correctamente con **.NET 10 dentro de Docker**.

---

## Material conceptual del curso

Además del código, el repositorio conserva documentación conceptual relacionada con los temas trabajados en clase.

| Documento | Tema |
|---|---|
| `docs/FLUJO_DE_UNA_PETICION.md` | Recorrido de una petición por las capas |
| `docs/PARADIGMA_POO.md` | Programación orientada a objetos |
| `docs/SOLID_CAPAS_PATRONES.md` | Principios SOLID y arquitectura por capas |
| `docs/PRINCIPIOS_ACID.md` | Propiedades ACID de una base de datos |
| `docs/PROGRAMACION_ASINCRONICA.md` | Programación asíncrona |
| `docs/CONCEPTOS_DOCKER.md` | Imágenes, contenedores, volúmenes y Compose |
| `docs/CALIDAD_DE_PRUEBAS.md` | Calidad y cobertura de pruebas |
| `docs/SDD_SPECKIT.md` | Desarrollo dirigido por especificaciones |

---

## Versiones

### v1

Construye la base del proyecto y los CRUD de los recursos sin claves foráneas.

### v2

Agrega los recursos relacionados, valida las claves foráneas, mantiene borrado lógico e incorpora las pantallas Blazor correspondientes.

**Estado: completada.**

### Siguientes versiones

El archivo `docs/spec_kit/versiones/0_mapa_versiones.md` define las siguientes etapas del proyecto, incluyendo autenticación y funcionalidades posteriores.

---

## Explicación rápida del proyecto

Una forma sencilla de presentar el proyecto es:

> Gestión Profesoral es una aplicación web que organiza la información académica y profesional de los docentes y permite relacionar datos como estudios, evaluaciones, experiencia, redes, apoyos y becas. El usuario trabaja desde Blazor, la API valida y procesa cada solicitud mediante una arquitectura por capas, y SQL Server almacena la información. La v1 construye los recursos independientes y la v2 incorpora las relaciones entre las diferentes tablas, con validaciones y borrado lógico. Todo el sistema puede ejecutarse con Docker Compose.

---

## Información académica

| Dato | Información |
|---|---|
| **Estudiante** | Veronica Osorio Durango |
| **Correo institucional** | veronicaosorio312028@correo.itm.edu.co |
| **Institución** | Instituto Tecnológico Metropolitano — ITM |
| **Asignatura** | Aplicación y Servicios Web |
| **Profesor** | Carlos Arturo Castro Castro |
| **Proyecto de aula** | Gestión Profesoral |
| **Versión entregada** | v2 |

---

<div align="center">

**Gestión Profesoral — Versión 2 completada**

Instituto Tecnológico Metropolitano — ITM  
Aplicación y Servicios Web  
Veronica Osorio Durango

</div>
