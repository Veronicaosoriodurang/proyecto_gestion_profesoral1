# Plan de implementación — V2

Se conserva Controller → IServicio → Servicio → IRepositorio → RepositorioSqlServer. Blazor consume únicamente la API. Los contratos de v1 se conservan; los cinco repositorios se migran a procedimientos porque V2 exige SQL almacenado en todas las operaciones.

## Bloques

1. Revisar código, documentos y requisitos del profesor. Contrastar el main remoto con el checkout local y recuperar avances. Crear rama de trabajo basada en el main observado.
2. Especificar el delta. Escribir migración no destructiva, 80 procedimientos CRUD, reglas de relaciones activas/retiro, procedimientos auxiliares de consulta de retiro y trigger. Adaptar repositorios y errores HTTP sin cambiar rutas.
3. Implementar expedientes concretos de docente/estudio, lecturas filtradas por maestro y escrituras con una conexión/transacción serializable. Validar cuerpos y reglas de fechas antes de escribir. SQL refuerza relaciones dentro de la transacción.
4. Conservar las once pantallas individuales; agregar detalles dentro del maestro con borradores y un botón de envío conjunto. Las relaciones adicionales se eligen de catálogos de API; el padre contextual lo fija el maestro.
5. Compilar, ejecutar prueba heredada de capas, matriz HTTP de los 16 recursos, eventos del trigger y recorridos en Chromium. Registrar resultados reales, correcciones y límites. Completar documentos, colección y prompts.

## Regresión y cierre

Los errores esperados son 400/404/409/422, nunca 500 por integridad o duplicados. Verificar identidades generadas, opcionales nulos, ambas claves de los puentes, maestro con detalle activo, retiro lógico y filtrado. Probar fallos intermedios tanto con maestro nuevo como existente y comprobar que el trigger también se revierte.

La comprobación automática no firma el checklist humano. La revisión individual, la revisión del PR y el tag v2 corresponden al cierre del equipo. No se implementan recursos de v3/v4.
