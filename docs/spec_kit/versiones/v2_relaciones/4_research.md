# Decisiones previas a implementar — 16 de octubre de 2026

Fuente: requisitos VERSION_2.md del profesor, consultados por el conector GitHub.
GitHub main observado: 7b94375e578333ca6c77dd7cf303acdc7cbcbcd6.
La copia local partía de 85613cb. Se recuperaron los avances remotos antes de editar.
No se encontraron procedimientos de área/término ni trg_contar_estudios en el árbol remoto ni en la copia local. No se supone que el trabajo no publicado exista en este entorno.

Se enmienda la regla de SQL directo de v1: el SQL pasa a procedimientos explícitos por recurso, conservando interfaces, rutas y borrado lógico. La migración v2 es idempotente y no recrea tablas ni semillas. Los nombres históricos experiecia/nacionalidaad se conservan.

Regla de retiro: se rechaza con 409 retirar cualquier maestro con hijos activos. Se comprueba en servicio y se refuerza dentro de la transacción de SQL para evitar carreras. Una pareja retirada conserva su clave; repetirla da 409, no se reactiva silenciosamente.

La transacción de expediente usa una sola conexión y transacción; todos sus renglones llaman los mismos procedimientos CRUD. Un error revierte también los efectos del trigger. El cliente no envía total_estudios.

No se etiqueta v2 hasta ejecutar los doce criterios, incluida regresión e interacción de Blazor contra SQL Server. Las limitaciones se registrarán sin inventar resultados.
