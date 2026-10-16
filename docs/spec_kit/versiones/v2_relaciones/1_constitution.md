# Constitución aplicable a V2

Se mantiene la [constitución del proyecto](../../1_constitution.md): C#/.NET 10, SQL Server, Dapper, capas explícitas por recurso y Blazor por HTTP. No hay entidades, controladores ni pantallas de acceso de v3 ni consultas/dashboard de v4.

Enmienda V2 (16 de octubre de 2026): por requisito del profesor, las operaciones SQL de producción pasan a procedimientos almacenados versionados en `db/version_2.sql`. El repositorio contiene únicamente llamadas parametrizadas a procedimientos. El SQL directo se permite en pruebas SQL para verificar el trigger y la persistencia física, nunca en repositorios de producción. La justificación y compatibilidad están en `4_research.md`.
