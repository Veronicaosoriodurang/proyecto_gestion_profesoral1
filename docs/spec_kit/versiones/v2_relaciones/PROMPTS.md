# Prompts reales y correcciones de esta continuación

Fecha: 16 de octubre de 2026. Herramienta: Codex. No se atribuyen estos cambios a integrantes ni se inventan prompts individuales. Cada integrante debe adjuntar sus prompts reales adicionales y preparar la sustentación.

## Petición recibida del usuario

> Continúa el desarrollo de mi proyecto universitario Gestión Profesoral, específicamente la VERSIÓN 2.
>
> Repositorio: Veronicaosoriodurang/proyecto_gestion_profesoral1.
>
> Primero revisa el código existente y los documentos del proyecto. Consulta también los requisitos del profesor en:
> https://github.com/ccastro2050/proyecto_aplicacion_y_servicios_web4/blob/main/ProyectosDeAula/docs/VERSION_2.md
>
> Objetivo: completar la versión 2 sin dañar ninguna funcionalidad de la versión 1.
>
> Requisitos principales:
>
> 1. Mantener funcionando el CRUD de las cinco tablas de la versión 1.
> 2. Implementar los CRUD pendientes de las tablas con claves foráneas de la versión 2, tanto en la API como en Blazor.
> 3. Utilizar procedimientos almacenados de SQL Server para todos los CRUD, con nombres sp_listar_, sp_consultar_, sp_crear_, sp_actualizar_ y sp_eliminar_.
> 4. Completar los formularios con listas desplegables para las claves foráneas.
> 5. Manejar errores de relaciones y duplicados con respuestas HTTP 409 en español.
> 6. Implementar el guardado atómico de registros maestro-detalle y las tablas puente.
> 7. Conservar y verificar el trigger trg_contar_estudios.
> 8. Completar los documentos Spec Kit y las pruebas que exige la versión 2.
>
> Ya existen avances locales: procedimientos de area_conocimiento y termino_clave, y un trigger de estudios. Comprueba cuáles están presentes en GitHub antes de modificar nada.
>
> Trabaja en una rama nueva para evitar afectar main. Implementa los cambios por bloques, ejecuta las pruebas disponibles y documenta lo que no puedas verificar.
>
> No implementes funcionalidades de las versiones 3 o 4. No elimines datos existentes ni inventes resultados de pruebas.
>
> Quiero que realices cambios reales en el código, no solamente recomendaciones. Al finalizar, indícame qué modificaste, qué pruebas pasaron y qué falta.

## Correcciones efectivamente realizadas durante la ejecución

- La copia local era más antigua que main: se consultó el árbol remoto y se recuperaron las once implementaciones existentes antes de continuar. Al recuperar acceso de terminal se obtuvo el historial y se basaron los commits propios directamente en main 7b94375. No había procedimientos ni trigger en la versión remota observada; no se supuso que los avances locales de otra máquina estuvieran disponibles.
- La compilación detectó el nombre heredado `ObtenerPorTermino`, distinto de `ObtenerPorId`: se preservó su interfaz al migrar.
- Se preservó el caso válido de PUT vacío de término clave: su único atributo editable es opcional. La expectativa de prueba inicial, 422, era incorrecta; no se cambió la funcionalidad de v1 para acomodar esa prueba.
- La revisión del contrato detectó que los recursos identity deben devolver el identificador generado, no una constante de filas afectadas. Se corrigieron evaluación, reconocimiento y experiencia y se agregó la comprobación de respuesta persistida para las 16 tablas.
- Para probar Blazor se usó el Chromium del entorno y el modo de desarrollo, igual al Dockerfile. Ejecutar un binario sin publicar en Production no habilitaba los recursos estáticos del SDK.
- Se ajustaron los selectores de la prueba interactiva para esperar la hidratación/carga y evitar coincidencia exacta sobre etiquetas de select que incluyen el texto de sus opciones. No se interpretó un fallo del selector como evidencia de éxito del producto.

No se realizó una regeneración independiente del código con otra IA. Esa comprobación académica queda pendiente y no se reemplaza por este registro.
