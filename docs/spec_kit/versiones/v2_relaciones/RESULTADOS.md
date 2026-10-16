# Resultados reales — V2, 16 de octubre de 2026

Rama: `desarrollo/version-2`. Base de código: main `7b94375e578333ca6c77dd7cf303acdc7cbcbcd6`. Se revisó VERSION_2.md del profesor con el conector GitHub. Los avances remotos se conservaron: ya existían once CRUD/pantallas V2, pero no procedimientos ni trigger en el script remoto observado. No se accedió a archivos locales de otra máquina.

## Qué se implementó

- Los cinco recursos V1 y once V2 llaman procedimientos explícitos para listar, consultar, crear, actualizar y retirar. Los contratos anteriores permanecen; POST agrega la fila persistida y las identidades generadas.
- `db/version_2.sql`: 80 procedimientos CRUD, consultas auxiliares de retiro, validación transaccional de padres activos y trigger de total_estudios. Migración idempotente sin recrear tablas/semillas. Admite también una columna total_estudios previa nullable y sin default.
- Relaciones inexistentes/inactivas, parejas duplicadas y retiro de maestro con hijos activos producen 409 en español. Los servicios validan relaciones/retiro; SQL refuerza las reglas frente a concurrencia.
- Expedientes de docente y estudio: una sola conexión/transacción serializable para la ficha y todos los renglones. Un fallo revierte también cambios de la ficha y del trigger. Los puentes usan ambas claves.
- Blazor conserva pantallas individuales y agrega detalle dentro del maestro, borradores visibles y envío conjunto. Las opciones muestran nombres obtenidos por API y envían claves; la relación opcional envía null. La creación de maestros usa el expediente incluso sin detalles.
- Nueve documentos V2, guía, prompt real recibido, correcciones, colección Postman y pruebas reproducibles.

## Pruebas ejecutadas

| Comprobación | Resultado y evidencia |
|---|---|
| API .NET 10 | Compila con 0 errores y 0 advertencias |
| Blazor .NET 10 | Compila con 0 errores y 0 advertencias |
| Prueba heredada de capas | 10 comprobaciones OK, sin base de datos |
| Matriz HTTP real | **401/401** comprobaciones, **308 peticiones**; `pruebas_v2/evidencias/integracion.json` |
| Expediente en Chromium/Blazor | **27/27**, incluyendo respuesta de las 16 pantallas, rollback por duplicado, dos estudios juntos, relación opcional null, área desde estudio y contador tras retiros; `evidencias/interfaz.json` |
| Regresión visual V1 | **20/20**: crear, guardar completo, guardar parcial y retirar en cada una de las cinco pantallas; `evidencias/regresion_interfaz_v1.json` |
| Trigger SQL | INSERT/DELETE multirregistro, cambio de padre, retiro y reactivación comprobados; datos sintéticos revertidos |
| Persistencia lógica | Las 16 filas retiradas por la matriz HTTP siguen físicamente en SQL Server con activo=0 |
| Reaplicar migración | Conteos totales/activos idénticos antes y después, incluso al normalizar una columna previa nullable/sin default; `evidencias/migracion_antes.txt`, `migracion_despues.txt` y `sql_resultados.txt` |
| Estructura | **125** comprobaciones: procedimientos, repositorios sin SQL directo, documentación y trigger; no sustituyen ejecución SQL |

Entorno utilizado: .NET SDK 10.0.402, SQL Server 2022 en un contenedor exclusivo, Python/Playwright y Chromium. La validación final se ejecutó sobre una base nueva `gestion_v2_validacion`, separada de los intentos preliminares. No se utilizó una base existente del usuario. Los JSON conservan fechas UTC del momento real de ejecución.

## Fallos encontrados y corregidos

La compilación inicial detectó la firma heredada ObtenerPorTermino; se conservó. La revisión encontró el retorno incorrecto de identidades en evaluación/reconocimiento/experiencia y se agregó regresión. Las pruebas iniciales también detectaron expectativas/selecciones incorrectas del propio guion: PUT vacío válido del término opcional, selector exacto de etiqueta de select y arranque de un binario Blazor sin recursos estáticos en Production. Esos intentos no se presentan como pruebas pasadas; las evidencias guardadas corresponden a la corrida final.

## Qué falta y qué no se afirma

- El equipo debe revisar/firmar `9_checklist.md` y repetir los doce criterios el día de entrega. La prueba visual cubre los cinco CRUD V1 y el flujo compuesto de V2; no automatiza cada combinación de edición de las once pantallas individuales, todas las resoluciones de pantalla ni concurrencia entre varios usuarios.
- Falta la sustentación oral individual y adjuntar otros prompts reales de los integrantes, si los hubo. No se hizo una regeneración independiente con otra IA.
- No se aplicó la migración a la base real de la universidad ni se desplegó el sistema del usuario. La conexión Docker completa en su máquina debe verificarse con el quickstart; aquí se ejecutaron los procesos y SQL Server localmente en el entorno aislado.
- No se integró a main ni se creó tag v2: corresponde hacerlo después de la revisión del equipo. No se implementaron recursos de V3/V4.

Las pruebas son específicas y reproducibles; no equivalen a afirmar ausencia absoluta de defectos.
