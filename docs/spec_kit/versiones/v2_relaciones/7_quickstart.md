# Ejecutar y verificar V2

## Base existente: conservar sus datos

No ejecutar `docker compose down -v` ni volver a ejecutar el script base sobre una base con datos. La migración que corresponde es **solo `db/version_2.sql`**. Con el compose del proyecto:

```bash
docker compose up -d sqlserver
docker compose run --rm sqlserver-init
docker compose up -d --build api-gestion front-blazor
```

`init.sh` crea/siembra únicamente si `gestion_local` no existe y aplica `version_2.sql` tanto a bases nuevas como existentes. Usa sqlcmd con `-b`: un error impide dar por exitosa la inicialización. La migración usa CREATE OR ALTER y comprueba si ya existe total_estudios. No elimina datos. Para otra instalación de SQL Server, ejecutar version_2.sql con sqlcmd `-d <base>` o seleccionar la base existente en SSMS.

API: http://localhost:8074/swagger · Blazor: http://localhost:8075. Las instrucciones Docker originales siguen aplicando; no se necesitan dependencias nuevas de la aplicación.

## Pruebas en una base desechable

1. Crear un SQL Server aislado, una base `gestion_v2_pruebas` y cargar el esquema dado **sin USE gestion_local ni la sección DROP**. Nunca usar una base real para pruebas.
2. Aplicar `db/version_2.sql`. Configurar `ConnectionStrings__SqlServer` de la API para esa base. Iniciar Blazor con `UrlApi` apuntando a la API de pruebas. Para ejecutar el binario sin publicar, usar `ASPNETCORE_ENVIRONMENT=Development` (los recursos estáticos de Blazor se resuelven desde el SDK).
3. Ejecutar:

```bash
dotnet build api_gestion
dotnet build front_blazor
dotnet run --project api_gestion/pruebas
python3 pruebas_v2/integracion.py --confirmar-base-pruebas
python3 pruebas_v2/interfaz.py --confirmar-base-pruebas --chromium /usr/bin/chromium
```

La prueba interactiva necesita Playwright para Python y Chromium. La API y Blazor deben estar ya iniciados. Los scripts aceptan `--api`, `--front` y `--salida`. El indicador confirma que el operador comprobó la base de destino; no sustituye esa comprobación. Las filas usan identificadores aleatorios y solo se retiran las creadas por la corrida.

4. Ejecutar `pruebas_v2/trigger.sql` con sqlcmd sobre gestion_v2_pruebas: prueba DML multirregistro y revierte sus datos. Reaplicar la migración y comprobar que conserva filas/conteos.
5. La colección Postman es una exportación de las peticiones reales de la corrida registrada: contiene expectativas positivas y negativas. Se reproduce en una base de pruebas nueva con semilla; sus claves fijas no se reutilizan tras un retiro lógico. El script Python genera una colección nueva con claves distintas en cada ejecución.

## Recorrido para sustentación

Abrir cada catálogo de v1 y crear/editar/retirar. Abrir Docentes → Agregar, llenar la ficha, dejar línea principal sin selección y abrir Estudios realizados. Preparar dos estudios, guardar el expediente y volver a editar: el total debe ser dos sin haber enviado ese campo. Probar dos estudios con el mismo código: no debe quedar ni el docente nuevo ni el primer estudio. Desde un estudio, preparar área, beca y apoyo; guardar juntos. Retirar un estudio sin detalle activo: el contador baja. Si tiene detalles activos, se deben retirar primero. Las pantallas individuales permiten editar/reemplazar y retirar cada recurso con sus claves originales.
