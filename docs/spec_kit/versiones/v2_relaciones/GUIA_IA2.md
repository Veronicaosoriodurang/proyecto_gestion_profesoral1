# Guía de IA — continuar V2

Leer constitución, mapa, documentos 1–9 de esta carpeta, código existente y VERSION_2.md del profesor. Preservar rutas, sobres, borrado lógico y catálogos de v1. Mantener recursos concretos con sus capas; los auxiliares compartidos solo manejan infraestructura (fechas, retiro y transporte). No agregar recursos de v3/v4.

El SQL de producción vive en procedimientos explícitos de db/version_2.sql. El repositorio parametriza llamadas con CommandType.StoredProcedure. Leer los procedimientos y los contratos antes de tocar una firma; actualizar ambos y las pruebas si cambia. La migración jamás recrea una base existente. No ejecutar el script base destructivo sobre datos reales.

Los comentarios deben explicar decisiones: por qué una conexión/transacción para todos los renglones, por qué el contador pertenece al trigger, por qué null no es una clave vacía y por qué 409 representa un conflicto. No afirmar que un comentario o compilación prueba el comportamiento.

Ejecutar pruebas reales sobre base aislada, guardar resultados exactos y reportar fallos/limitaciones. No firmar el checklist ni crear tag v2 con pruebas pendientes. Registrar prompts y correcciones reales en PROMPTS.md; no atribuir trabajo a integrantes ni inventar una prueba con otra IA.
