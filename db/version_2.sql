-- Migración V2: ejecutar sobre gestion_local existente. No elimina datos.

SET XACT_ABORT ON;
GO

IF COL_LENGTH('dbo.docente', 'total_estudios') IS NULL
    ALTER TABLE dbo.docente ADD total_estudios INT NOT NULL CONSTRAINT DF_docente_total_estudios DEFAULT 0;
GO

CREATE OR ALTER TRIGGER dbo.trg_contar_estudios ON dbo.estudios_realizados
AFTER INSERT, UPDATE, DELETE AS
BEGIN
 SET NOCOUNT ON;
 UPDATE d SET total_estudios = (SELECT COUNT(*) FROM dbo.estudios_realizados e WHERE e.docente=d.cedula AND e.activo=1)
 FROM dbo.docente d WHERE d.cedula IN (SELECT docente FROM inserted UNION SELECT docente FROM deleted);
END;
GO
-- Inicializa también los docentes que ya existían antes de instalar el trigger.
UPDATE d SET total_estudios=(SELECT COUNT(*) FROM dbo.estudios_realizados e WHERE e.docente=d.cedula AND e.activo=1) FROM dbo.docente d;
GO

CREATE OR ALTER PROCEDURE dbo.sp_listar_area_conocimiento
 @Limite INT = 1000
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT TOP (@Limite) id AS Id, gran_area AS GranArea, area AS Area, disciplina AS Disciplina FROM dbo.area_conocimiento WHERE activo=1 ORDER BY id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_area_conocimiento
 @Id VARCHAR(6)
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT id AS Id, gran_area AS GranArea, area AS Area, disciplina AS Disciplina FROM dbo.area_conocimiento WHERE id=@Id AND activo=1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_crear_area_conocimiento
 @Id VARCHAR(6),
 @GranArea VARCHAR(60),
 @Area VARCHAR(60),
 @Disciplina VARCHAR(150)
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;

INSERT INTO dbo.area_conocimiento (id,gran_area,area,disciplina,activo) VALUES (@Id,@GranArea,@Area,@Disciplina,1);
SELECT id AS Id, gran_area AS GranArea, area AS Area, disciplina AS Disciplina FROM dbo.area_conocimiento WHERE id=@Id AND activo=1;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_area_conocimiento
 @OriginalId VARCHAR(6),
 @Id VARCHAR(6) = NULL,
 @GranArea VARCHAR(60) = NULL,
 @Area VARCHAR(60) = NULL,
 @Disciplina VARCHAR(150) = NULL,
 @Parcial BIT=0
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF NOT EXISTS(SELECT 1 FROM dbo.area_conocimiento WITH(UPDLOCK,HOLDLOCK) WHERE id=@OriginalId AND activo=1) BEGIN SELECT 0; COMMIT TRANSACTION; RETURN; END;

UPDATE dbo.area_conocimiento SET id=CASE WHEN @Parcial=1 THEN COALESCE(@Id,id) ELSE @Id END,
 gran_area=CASE WHEN @Parcial=1 THEN COALESCE(@GranArea,gran_area) ELSE @GranArea END,
 area=CASE WHEN @Parcial=1 THEN COALESCE(@Area,area) ELSE @Area END,
 disciplina=CASE WHEN @Parcial=1 THEN COALESCE(@Disciplina,disciplina) ELSE @Disciplina END WHERE id=@OriginalId AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_area_conocimiento
 @Id VARCHAR(6)
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF EXISTS(SELECT 1 FROM dbo.estudio_ac WITH(UPDLOCK,HOLDLOCK) WHERE area_conocimiento=@Id AND activo=1) THROW 51002, N'Retire primero los registros relacionados activos.', 1;
UPDATE dbo.area_conocimiento SET activo=0 WHERE id=@Id AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_listar_termino_clave
 @Limite INT = 1000
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT TOP (@Limite) termino AS Termino, termino_ingles AS TerminoIngles FROM dbo.termino_clave WHERE activo=1 ORDER BY termino;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_termino_clave
 @Termino VARCHAR(30)
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT termino AS Termino, termino_ingles AS TerminoIngles FROM dbo.termino_clave WHERE termino=@Termino AND activo=1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_crear_termino_clave
 @Termino VARCHAR(30),
 @TerminoIngles VARCHAR(30)
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;

INSERT INTO dbo.termino_clave (termino,termino_ingles,activo) VALUES (@Termino,@TerminoIngles,1);
SELECT termino AS Termino, termino_ingles AS TerminoIngles FROM dbo.termino_clave WHERE termino=@Termino AND activo=1;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_termino_clave
 @OriginalTermino VARCHAR(30),
 @Termino VARCHAR(30) = NULL,
 @TerminoIngles VARCHAR(30) = NULL,
 @Parcial BIT=0
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF NOT EXISTS(SELECT 1 FROM dbo.termino_clave WITH(UPDLOCK,HOLDLOCK) WHERE termino=@OriginalTermino AND activo=1) BEGIN SELECT 0; COMMIT TRANSACTION; RETURN; END;

UPDATE dbo.termino_clave SET termino=CASE WHEN @Parcial=1 THEN COALESCE(@Termino,termino) ELSE @Termino END,
 termino_ingles=CASE WHEN @Parcial=1 THEN COALESCE(@TerminoIngles,termino_ingles) ELSE @TerminoIngles END WHERE termino=@OriginalTermino AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_termino_clave
 @Termino VARCHAR(30)
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF EXISTS(SELECT 1 FROM dbo.intereses_futuros WITH(UPDLOCK,HOLDLOCK) WHERE termino_clave=@Termino AND activo=1) THROW 51002, N'Retire primero los registros relacionados activos.', 1;
UPDATE dbo.termino_clave SET activo=0 WHERE termino=@Termino AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_listar_linea_investigacion
 @Limite INT = 1000
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT TOP (@Limite) id AS Id, nombre AS Nombre, descripcion AS Descripcion FROM dbo.linea_investigacion WHERE activo=1 ORDER BY id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_linea_investigacion
 @Id INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT id AS Id, nombre AS Nombre, descripcion AS Descripcion FROM dbo.linea_investigacion WHERE id=@Id AND activo=1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_crear_linea_investigacion
 @Nombre VARCHAR(45),
 @Descripcion VARCHAR(256)
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;

INSERT INTO dbo.linea_investigacion (nombre,descripcion,activo) VALUES (@Nombre,@Descripcion,1);
DECLARE @Id INT=CONVERT(INT,SCOPE_IDENTITY());
SELECT id AS Id, nombre AS Nombre, descripcion AS Descripcion FROM dbo.linea_investigacion WHERE id=@Id AND activo=1;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_linea_investigacion
 @OriginalId INT,
 @Nombre VARCHAR(45) = NULL,
 @Descripcion VARCHAR(256) = NULL,
 @Parcial BIT=0
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF NOT EXISTS(SELECT 1 FROM dbo.linea_investigacion WITH(UPDLOCK,HOLDLOCK) WHERE id=@OriginalId AND activo=1) BEGIN SELECT 0; COMMIT TRANSACTION; RETURN; END;

UPDATE dbo.linea_investigacion SET nombre=CASE WHEN @Parcial=1 THEN COALESCE(@Nombre,nombre) ELSE @Nombre END,
 descripcion=CASE WHEN @Parcial=1 THEN COALESCE(@Descripcion,descripcion) ELSE @Descripcion END WHERE id=@OriginalId AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_linea_investigacion
 @Id INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF EXISTS(SELECT 1 FROM dbo.docente WITH(UPDLOCK,HOLDLOCK) WHERE linea_investigacion_principal=@Id AND activo=1) THROW 51002, N'Retire primero los registros relacionados activos.', 1;
UPDATE dbo.linea_investigacion SET activo=0 WHERE id=@Id AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_listar_programa
 @Limite INT = 1000
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT TOP (@Limite) id AS Id, nombre AS Nombre, tipo AS Tipo, nivel AS Nivel, fecha_creacion AS FechaCreacion, fecha_cierre AS FechaCierre, numero_cohortes AS NumeroCohortes, cant_graduados AS CantGraduados, fecha_actualizacion AS FechaActualizacion, ciudad AS Ciudad, facultad AS Facultad FROM dbo.programa WHERE activo=1 ORDER BY id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_programa
 @Id INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT id AS Id, nombre AS Nombre, tipo AS Tipo, nivel AS Nivel, fecha_creacion AS FechaCreacion, fecha_cierre AS FechaCierre, numero_cohortes AS NumeroCohortes, cant_graduados AS CantGraduados, fecha_actualizacion AS FechaActualizacion, ciudad AS Ciudad, facultad AS Facultad FROM dbo.programa WHERE id=@Id AND activo=1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_crear_programa
 @Id INT,
 @Nombre VARCHAR(60),
 @Tipo VARCHAR(45),
 @Nivel VARCHAR(45),
 @FechaCreacion VARCHAR(45),
 @FechaCierre VARCHAR(45),
 @NumeroCohortes VARCHAR(45),
 @CantGraduados VARCHAR(45),
 @FechaActualizacion VARCHAR(45),
 @Ciudad VARCHAR(45),
 @Facultad INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;

INSERT INTO dbo.programa (id,nombre,tipo,nivel,fecha_creacion,fecha_cierre,numero_cohortes,cant_graduados,fecha_actualizacion,ciudad,facultad,activo) VALUES (@Id,@Nombre,@Tipo,@Nivel,@FechaCreacion,@FechaCierre,@NumeroCohortes,@CantGraduados,@FechaActualizacion,@Ciudad,@Facultad,1);
SELECT id AS Id, nombre AS Nombre, tipo AS Tipo, nivel AS Nivel, fecha_creacion AS FechaCreacion, fecha_cierre AS FechaCierre, numero_cohortes AS NumeroCohortes, cant_graduados AS CantGraduados, fecha_actualizacion AS FechaActualizacion, ciudad AS Ciudad, facultad AS Facultad FROM dbo.programa WHERE id=@Id AND activo=1;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_programa
 @OriginalId INT,
 @Id INT = NULL,
 @Nombre VARCHAR(60) = NULL,
 @Tipo VARCHAR(45) = NULL,
 @Nivel VARCHAR(45) = NULL,
 @FechaCreacion VARCHAR(45) = NULL,
 @FechaCierre VARCHAR(45) = NULL,
 @NumeroCohortes VARCHAR(45) = NULL,
 @CantGraduados VARCHAR(45) = NULL,
 @FechaActualizacion VARCHAR(45) = NULL,
 @Ciudad VARCHAR(45) = NULL,
 @Facultad INT = NULL,
 @Parcial BIT=0
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF NOT EXISTS(SELECT 1 FROM dbo.programa WITH(UPDLOCK,HOLDLOCK) WHERE id=@OriginalId AND activo=1) BEGIN SELECT 0; COMMIT TRANSACTION; RETURN; END;

UPDATE dbo.programa SET id=CASE WHEN @Parcial=1 THEN COALESCE(@Id,id) ELSE @Id END,
 nombre=CASE WHEN @Parcial=1 THEN COALESCE(@Nombre,nombre) ELSE @Nombre END,
 tipo=CASE WHEN @Parcial=1 THEN COALESCE(@Tipo,tipo) ELSE @Tipo END,
 nivel=CASE WHEN @Parcial=1 THEN COALESCE(@Nivel,nivel) ELSE @Nivel END,
 fecha_creacion=CASE WHEN @Parcial=1 THEN COALESCE(@FechaCreacion,fecha_creacion) ELSE @FechaCreacion END,
 fecha_cierre=CASE WHEN @Parcial=1 THEN COALESCE(@FechaCierre,fecha_cierre) ELSE @FechaCierre END,
 numero_cohortes=CASE WHEN @Parcial=1 THEN COALESCE(@NumeroCohortes,numero_cohortes) ELSE @NumeroCohortes END,
 cant_graduados=CASE WHEN @Parcial=1 THEN COALESCE(@CantGraduados,cant_graduados) ELSE @CantGraduados END,
 fecha_actualizacion=CASE WHEN @Parcial=1 THEN COALESCE(@FechaActualizacion,fecha_actualizacion) ELSE @FechaActualizacion END,
 ciudad=CASE WHEN @Parcial=1 THEN COALESCE(@Ciudad,ciudad) ELSE @Ciudad END,
 facultad=CASE WHEN @Parcial=1 THEN COALESCE(@Facultad,facultad) ELSE @Facultad END WHERE id=@OriginalId AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_programa
 @Id INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF EXISTS(SELECT 1 FROM dbo.docente_departamento WITH(UPDLOCK,HOLDLOCK) WHERE departamento=@Id AND activo=1) THROW 51002, N'Retire primero los registros relacionados activos.', 1;
UPDATE dbo.programa SET activo=0 WHERE id=@Id AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_listar_red
 @Limite INT = 1000
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT TOP (@Limite) idr AS Idr, nombre AS Nombre, url AS Url, pais AS Pais FROM dbo.red WHERE activo=1 ORDER BY idr;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_red
 @Idr INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT idr AS Idr, nombre AS Nombre, url AS Url, pais AS Pais FROM dbo.red WHERE idr=@Idr AND activo=1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_crear_red
 @Idr INT,
 @Nombre VARCHAR(45),
 @Url VARCHAR(45),
 @Pais VARCHAR(45)
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;

INSERT INTO dbo.red (idr,nombre,url,pais,activo) VALUES (@Idr,@Nombre,@Url,@Pais,1);
SELECT idr AS Idr, nombre AS Nombre, url AS Url, pais AS Pais FROM dbo.red WHERE idr=@Idr AND activo=1;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_red
 @OriginalIdr INT,
 @Idr INT = NULL,
 @Nombre VARCHAR(45) = NULL,
 @Url VARCHAR(45) = NULL,
 @Pais VARCHAR(45) = NULL,
 @Parcial BIT=0
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF NOT EXISTS(SELECT 1 FROM dbo.red WITH(UPDLOCK,HOLDLOCK) WHERE idr=@OriginalIdr AND activo=1) BEGIN SELECT 0; COMMIT TRANSACTION; RETURN; END;

UPDATE dbo.red SET idr=CASE WHEN @Parcial=1 THEN COALESCE(@Idr,idr) ELSE @Idr END,
 nombre=CASE WHEN @Parcial=1 THEN COALESCE(@Nombre,nombre) ELSE @Nombre END,
 url=CASE WHEN @Parcial=1 THEN COALESCE(@Url,url) ELSE @Url END,
 pais=CASE WHEN @Parcial=1 THEN COALESCE(@Pais,pais) ELSE @Pais END WHERE idr=@OriginalIdr AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_red
 @Idr INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF EXISTS(SELECT 1 FROM dbo.red_docente WITH(UPDLOCK,HOLDLOCK) WHERE red=@Idr AND activo=1) THROW 51002, N'Retire primero los registros relacionados activos.', 1;
UPDATE dbo.red SET activo=0 WHERE idr=@Idr AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_listar_docente
 @Limite INT = 1000
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT TOP (@Limite) cedula AS Cedula, nombres AS Nombres, apellidos AS Apellidos, genero AS Genero, cargo AS Cargo, fecha_nacimiento AS FechaNacimiento, correo AS Correo, telefono AS Telefono, url_cvlac AS UrlCvlac, fecha_actualizacion AS FechaActualizacion, escalafon AS Escalafon, perfil AS Perfil, cat_minciencia AS CatMinciencia, conv_minciencia AS ConvMinciencia, nacionalidaad AS Nacionalidaad, linea_investigacion_principal AS LineaInvestigacionPrincipal, total_estudios AS TotalEstudios FROM dbo.docente WHERE activo=1 ORDER BY cedula;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_docente
 @Cedula INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT cedula AS Cedula, nombres AS Nombres, apellidos AS Apellidos, genero AS Genero, cargo AS Cargo, fecha_nacimiento AS FechaNacimiento, correo AS Correo, telefono AS Telefono, url_cvlac AS UrlCvlac, fecha_actualizacion AS FechaActualizacion, escalafon AS Escalafon, perfil AS Perfil, cat_minciencia AS CatMinciencia, conv_minciencia AS ConvMinciencia, nacionalidaad AS Nacionalidaad, linea_investigacion_principal AS LineaInvestigacionPrincipal, total_estudios AS TotalEstudios FROM dbo.docente WHERE cedula=@Cedula AND activo=1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_crear_docente
 @Cedula INT,
 @Nombres VARCHAR(60),
 @Apellidos VARCHAR(60),
 @Genero VARCHAR(12),
 @Cargo VARCHAR(30),
 @FechaNacimiento DATE,
 @Correo VARCHAR(70),
 @Telefono VARCHAR(20),
 @UrlCvlac VARCHAR(128),
 @FechaActualizacion DATE,
 @Escalafon VARCHAR(45),
 @Perfil VARCHAR(MAX),
 @CatMinciencia VARCHAR(45),
 @ConvMinciencia VARCHAR(45),
 @Nacionalidaad VARCHAR(45),
 @LineaInvestigacionPrincipal INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF @LineaInvestigacionPrincipal IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.linea_investigacion WITH (UPDLOCK,HOLDLOCK) WHERE id=@LineaInvestigacionPrincipal AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
INSERT INTO dbo.docente (cedula,nombres,apellidos,genero,cargo,fecha_nacimiento,correo,telefono,url_cvlac,fecha_actualizacion,escalafon,perfil,cat_minciencia,conv_minciencia,nacionalidaad,linea_investigacion_principal,activo) VALUES (@Cedula,@Nombres,@Apellidos,@Genero,@Cargo,@FechaNacimiento,@Correo,@Telefono,@UrlCvlac,@FechaActualizacion,@Escalafon,@Perfil,@CatMinciencia,@ConvMinciencia,@Nacionalidaad,@LineaInvestigacionPrincipal,1);
SELECT cedula AS Cedula, nombres AS Nombres, apellidos AS Apellidos, genero AS Genero, cargo AS Cargo, fecha_nacimiento AS FechaNacimiento, correo AS Correo, telefono AS Telefono, url_cvlac AS UrlCvlac, fecha_actualizacion AS FechaActualizacion, escalafon AS Escalafon, perfil AS Perfil, cat_minciencia AS CatMinciencia, conv_minciencia AS ConvMinciencia, nacionalidaad AS Nacionalidaad, linea_investigacion_principal AS LineaInvestigacionPrincipal, total_estudios AS TotalEstudios FROM dbo.docente WHERE cedula=@Cedula AND activo=1;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_docente
 @OriginalCedula INT,
 @Cedula INT = NULL,
 @Nombres VARCHAR(60) = NULL,
 @Apellidos VARCHAR(60) = NULL,
 @Genero VARCHAR(12) = NULL,
 @Cargo VARCHAR(30) = NULL,
 @FechaNacimiento DATE = NULL,
 @Correo VARCHAR(70) = NULL,
 @Telefono VARCHAR(20) = NULL,
 @UrlCvlac VARCHAR(128) = NULL,
 @FechaActualizacion DATE = NULL,
 @Escalafon VARCHAR(45) = NULL,
 @Perfil VARCHAR(MAX) = NULL,
 @CatMinciencia VARCHAR(45) = NULL,
 @ConvMinciencia VARCHAR(45) = NULL,
 @Nacionalidaad VARCHAR(45) = NULL,
 @LineaInvestigacionPrincipal INT = NULL,
 @Parcial BIT=0
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF NOT EXISTS(SELECT 1 FROM dbo.docente WITH(UPDLOCK,HOLDLOCK) WHERE cedula=@OriginalCedula AND activo=1) BEGIN SELECT 0; COMMIT TRANSACTION; RETURN; END;
IF @LineaInvestigacionPrincipal IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.linea_investigacion WITH (UPDLOCK,HOLDLOCK) WHERE id=@LineaInvestigacionPrincipal AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
UPDATE dbo.docente SET cedula=CASE WHEN @Parcial=1 THEN COALESCE(@Cedula,cedula) ELSE @Cedula END,
 nombres=CASE WHEN @Parcial=1 THEN COALESCE(@Nombres,nombres) ELSE @Nombres END,
 apellidos=CASE WHEN @Parcial=1 THEN COALESCE(@Apellidos,apellidos) ELSE @Apellidos END,
 genero=CASE WHEN @Parcial=1 THEN COALESCE(@Genero,genero) ELSE @Genero END,
 cargo=CASE WHEN @Parcial=1 THEN COALESCE(@Cargo,cargo) ELSE @Cargo END,
 fecha_nacimiento=CASE WHEN @Parcial=1 THEN COALESCE(@FechaNacimiento,fecha_nacimiento) ELSE @FechaNacimiento END,
 correo=CASE WHEN @Parcial=1 THEN COALESCE(@Correo,correo) ELSE @Correo END,
 telefono=CASE WHEN @Parcial=1 THEN COALESCE(@Telefono,telefono) ELSE @Telefono END,
 url_cvlac=CASE WHEN @Parcial=1 THEN COALESCE(@UrlCvlac,url_cvlac) ELSE @UrlCvlac END,
 fecha_actualizacion=CASE WHEN @Parcial=1 THEN COALESCE(@FechaActualizacion,fecha_actualizacion) ELSE @FechaActualizacion END,
 escalafon=CASE WHEN @Parcial=1 THEN COALESCE(@Escalafon,escalafon) ELSE @Escalafon END,
 perfil=CASE WHEN @Parcial=1 THEN COALESCE(@Perfil,perfil) ELSE @Perfil END,
 cat_minciencia=CASE WHEN @Parcial=1 THEN COALESCE(@CatMinciencia,cat_minciencia) ELSE @CatMinciencia END,
 conv_minciencia=CASE WHEN @Parcial=1 THEN COALESCE(@ConvMinciencia,conv_minciencia) ELSE @ConvMinciencia END,
 nacionalidaad=CASE WHEN @Parcial=1 THEN COALESCE(@Nacionalidaad,nacionalidaad) ELSE @Nacionalidaad END,
 linea_investigacion_principal=CASE WHEN @Parcial=1 THEN COALESCE(@LineaInvestigacionPrincipal,linea_investigacion_principal) ELSE @LineaInvestigacionPrincipal END WHERE cedula=@OriginalCedula AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_docente
 @Cedula INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF EXISTS(SELECT 1 FROM dbo.estudios_realizados WITH(UPDLOCK,HOLDLOCK) WHERE docente=@Cedula AND activo=1) THROW 51002, N'Retire primero los registros relacionados activos.', 1;
IF EXISTS(SELECT 1 FROM dbo.docente_departamento WITH(UPDLOCK,HOLDLOCK) WHERE docente=@Cedula AND activo=1) THROW 51002, N'Retire primero los registros relacionados activos.', 1;
IF EXISTS(SELECT 1 FROM dbo.intereses_futuros WITH(UPDLOCK,HOLDLOCK) WHERE docente=@Cedula AND activo=1) THROW 51002, N'Retire primero los registros relacionados activos.', 1;
IF EXISTS(SELECT 1 FROM dbo.evaluacion_docente WITH(UPDLOCK,HOLDLOCK) WHERE docente=@Cedula AND activo=1) THROW 51002, N'Retire primero los registros relacionados activos.', 1;
IF EXISTS(SELECT 1 FROM dbo.reconocimiento WITH(UPDLOCK,HOLDLOCK) WHERE docente=@Cedula AND activo=1) THROW 51002, N'Retire primero los registros relacionados activos.', 1;
IF EXISTS(SELECT 1 FROM dbo.experiecia WITH(UPDLOCK,HOLDLOCK) WHERE docente=@Cedula AND activo=1) THROW 51002, N'Retire primero los registros relacionados activos.', 1;
IF EXISTS(SELECT 1 FROM dbo.red_docente WITH(UPDLOCK,HOLDLOCK) WHERE docente=@Cedula AND activo=1) THROW 51002, N'Retire primero los registros relacionados activos.', 1;
UPDATE dbo.docente SET activo=0 WHERE cedula=@Cedula AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_listar_estudios_realizados
 @Limite INT = 1000, @Maestro INT = NULL
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT TOP (@Limite) id AS Id, titulo AS Titulo, universidad AS Universidad, fecha AS Fecha, tipo AS Tipo, ciudad AS Ciudad, docente AS Docente, ins_acreditada AS InsAcreditada, metodologia AS Metodologia, perfil_egresado AS PerfilEgresado, pais AS Pais FROM dbo.estudios_realizados WHERE activo=1 AND (@Maestro IS NULL OR docente=@Maestro) ORDER BY id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_estudios_realizados
 @Id INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT id AS Id, titulo AS Titulo, universidad AS Universidad, fecha AS Fecha, tipo AS Tipo, ciudad AS Ciudad, docente AS Docente, ins_acreditada AS InsAcreditada, metodologia AS Metodologia, perfil_egresado AS PerfilEgresado, pais AS Pais FROM dbo.estudios_realizados WHERE id=@Id AND activo=1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_crear_estudios_realizados
 @Id INT,
 @Titulo VARCHAR(45),
 @Universidad VARCHAR(50),
 @Fecha DATE,
 @Tipo VARCHAR(45),
 @Ciudad VARCHAR(45),
 @Docente INT,
 @InsAcreditada TINYINT,
 @Metodologia VARCHAR(45),
 @PerfilEgresado VARCHAR(MAX),
 @Pais VARCHAR(45)
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF @Docente IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.docente WITH (UPDLOCK,HOLDLOCK) WHERE cedula=@Docente AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
INSERT INTO dbo.estudios_realizados (id,titulo,universidad,fecha,tipo,ciudad,docente,ins_acreditada,metodologia,perfil_egresado,pais,activo) VALUES (@Id,@Titulo,@Universidad,@Fecha,@Tipo,@Ciudad,@Docente,@InsAcreditada,@Metodologia,@PerfilEgresado,@Pais,1);
SELECT id AS Id, titulo AS Titulo, universidad AS Universidad, fecha AS Fecha, tipo AS Tipo, ciudad AS Ciudad, docente AS Docente, ins_acreditada AS InsAcreditada, metodologia AS Metodologia, perfil_egresado AS PerfilEgresado, pais AS Pais FROM dbo.estudios_realizados WHERE id=@Id AND activo=1;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_estudios_realizados
 @OriginalId INT,
 @Id INT = NULL,
 @Titulo VARCHAR(45) = NULL,
 @Universidad VARCHAR(50) = NULL,
 @Fecha DATE = NULL,
 @Tipo VARCHAR(45) = NULL,
 @Ciudad VARCHAR(45) = NULL,
 @Docente INT = NULL,
 @InsAcreditada TINYINT = NULL,
 @Metodologia VARCHAR(45) = NULL,
 @PerfilEgresado VARCHAR(MAX) = NULL,
 @Pais VARCHAR(45) = NULL,
 @Parcial BIT=0
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF NOT EXISTS(SELECT 1 FROM dbo.estudios_realizados WITH(UPDLOCK,HOLDLOCK) WHERE id=@OriginalId AND activo=1) BEGIN SELECT 0; COMMIT TRANSACTION; RETURN; END;
IF @Docente IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.docente WITH (UPDLOCK,HOLDLOCK) WHERE cedula=@Docente AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
UPDATE dbo.estudios_realizados SET id=CASE WHEN @Parcial=1 THEN COALESCE(@Id,id) ELSE @Id END,
 titulo=CASE WHEN @Parcial=1 THEN COALESCE(@Titulo,titulo) ELSE @Titulo END,
 universidad=CASE WHEN @Parcial=1 THEN COALESCE(@Universidad,universidad) ELSE @Universidad END,
 fecha=CASE WHEN @Parcial=1 THEN COALESCE(@Fecha,fecha) ELSE @Fecha END,
 tipo=CASE WHEN @Parcial=1 THEN COALESCE(@Tipo,tipo) ELSE @Tipo END,
 ciudad=CASE WHEN @Parcial=1 THEN COALESCE(@Ciudad,ciudad) ELSE @Ciudad END,
 docente=CASE WHEN @Parcial=1 THEN COALESCE(@Docente,docente) ELSE @Docente END,
 ins_acreditada=CASE WHEN @Parcial=1 THEN COALESCE(@InsAcreditada,ins_acreditada) ELSE @InsAcreditada END,
 metodologia=CASE WHEN @Parcial=1 THEN COALESCE(@Metodologia,metodologia) ELSE @Metodologia END,
 perfil_egresado=CASE WHEN @Parcial=1 THEN COALESCE(@PerfilEgresado,perfil_egresado) ELSE @PerfilEgresado END,
 pais=CASE WHEN @Parcial=1 THEN COALESCE(@Pais,pais) ELSE @Pais END WHERE id=@OriginalId AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_estudios_realizados
 @Id INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF EXISTS(SELECT 1 FROM dbo.estudio_ac WITH(UPDLOCK,HOLDLOCK) WHERE estudio=@Id AND activo=1) THROW 51002, N'Retire primero los registros relacionados activos.', 1;
IF EXISTS(SELECT 1 FROM dbo.apoyo_profesoral WITH(UPDLOCK,HOLDLOCK) WHERE estudios=@Id AND activo=1) THROW 51002, N'Retire primero los registros relacionados activos.', 1;
IF EXISTS(SELECT 1 FROM dbo.beca WITH(UPDLOCK,HOLDLOCK) WHERE estudios=@Id AND activo=1) THROW 51002, N'Retire primero los registros relacionados activos.', 1;
UPDATE dbo.estudios_realizados SET activo=0 WHERE id=@Id AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_listar_docente_departamento
 @Limite INT = 1000, @Maestro INT = NULL
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT TOP (@Limite) docente AS Docente, departamento AS Departamento, dedicacion AS Dedicacion, modalidad AS Modalidad, fecha_ingreso AS FechaIngreso, fecha_salida AS FechaSalida FROM dbo.docente_departamento WHERE activo=1 AND (@Maestro IS NULL OR docente=@Maestro) ORDER BY docente,departamento;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_docente_departamento
 @Docente INT,
 @Departamento INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT docente AS Docente, departamento AS Departamento, dedicacion AS Dedicacion, modalidad AS Modalidad, fecha_ingreso AS FechaIngreso, fecha_salida AS FechaSalida FROM dbo.docente_departamento WHERE docente=@Docente AND departamento=@Departamento AND activo=1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_crear_docente_departamento
 @Docente INT,
 @Departamento INT,
 @Dedicacion VARCHAR(15),
 @Modalidad VARCHAR(45),
 @FechaIngreso DATE,
 @FechaSalida DATE
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF @Docente IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.docente WITH (UPDLOCK,HOLDLOCK) WHERE cedula=@Docente AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
IF @Departamento IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.programa WITH (UPDLOCK,HOLDLOCK) WHERE id=@Departamento AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
INSERT INTO dbo.docente_departamento (docente,departamento,dedicacion,modalidad,fecha_ingreso,fecha_salida,activo) VALUES (@Docente,@Departamento,@Dedicacion,@Modalidad,@FechaIngreso,@FechaSalida,1);
SELECT docente AS Docente, departamento AS Departamento, dedicacion AS Dedicacion, modalidad AS Modalidad, fecha_ingreso AS FechaIngreso, fecha_salida AS FechaSalida FROM dbo.docente_departamento WHERE docente=@Docente AND departamento=@Departamento AND activo=1;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_docente_departamento
 @OriginalDocente INT,
 @OriginalDepartamento INT,
 @Docente INT = NULL,
 @Departamento INT = NULL,
 @Dedicacion VARCHAR(15) = NULL,
 @Modalidad VARCHAR(45) = NULL,
 @FechaIngreso DATE = NULL,
 @FechaSalida DATE = NULL,
 @Parcial BIT=0
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF NOT EXISTS(SELECT 1 FROM dbo.docente_departamento WITH(UPDLOCK,HOLDLOCK) WHERE docente=@OriginalDocente AND departamento=@OriginalDepartamento AND activo=1) BEGIN SELECT 0; COMMIT TRANSACTION; RETURN; END;
IF @Docente IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.docente WITH (UPDLOCK,HOLDLOCK) WHERE cedula=@Docente AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
IF @Departamento IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.programa WITH (UPDLOCK,HOLDLOCK) WHERE id=@Departamento AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
UPDATE dbo.docente_departamento SET docente=CASE WHEN @Parcial=1 THEN COALESCE(@Docente,docente) ELSE @Docente END,
 departamento=CASE WHEN @Parcial=1 THEN COALESCE(@Departamento,departamento) ELSE @Departamento END,
 dedicacion=CASE WHEN @Parcial=1 THEN COALESCE(@Dedicacion,dedicacion) ELSE @Dedicacion END,
 modalidad=CASE WHEN @Parcial=1 THEN COALESCE(@Modalidad,modalidad) ELSE @Modalidad END,
 fecha_ingreso=CASE WHEN @Parcial=1 THEN COALESCE(@FechaIngreso,fecha_ingreso) ELSE @FechaIngreso END,
 fecha_salida=CASE WHEN @Parcial=1 THEN COALESCE(@FechaSalida,fecha_salida) ELSE @FechaSalida END WHERE docente=@OriginalDocente AND departamento=@OriginalDepartamento AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_docente_departamento
 @Docente INT,
 @Departamento INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;

UPDATE dbo.docente_departamento SET activo=0 WHERE docente=@Docente AND departamento=@Departamento AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_listar_intereses_futuros
 @Limite INT = 1000, @Maestro INT = NULL
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT TOP (@Limite) docente AS Docente, termino_clave AS TerminoClave FROM dbo.intereses_futuros WHERE activo=1 AND (@Maestro IS NULL OR docente=@Maestro) ORDER BY docente,termino_clave;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_intereses_futuros
 @Docente INT,
 @TerminoClave VARCHAR(30)
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT docente AS Docente, termino_clave AS TerminoClave FROM dbo.intereses_futuros WHERE docente=@Docente AND termino_clave=@TerminoClave AND activo=1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_crear_intereses_futuros
 @Docente INT,
 @TerminoClave VARCHAR(30)
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF @Docente IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.docente WITH (UPDLOCK,HOLDLOCK) WHERE cedula=@Docente AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
IF @TerminoClave IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.termino_clave WITH (UPDLOCK,HOLDLOCK) WHERE termino=@TerminoClave AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
INSERT INTO dbo.intereses_futuros (docente,termino_clave,activo) VALUES (@Docente,@TerminoClave,1);
SELECT docente AS Docente, termino_clave AS TerminoClave FROM dbo.intereses_futuros WHERE docente=@Docente AND termino_clave=@TerminoClave AND activo=1;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_intereses_futuros
 @OriginalDocente INT,
 @OriginalTerminoClave VARCHAR(30),
 @Docente INT = NULL,
 @TerminoClave VARCHAR(30) = NULL,
 @Parcial BIT=0
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF NOT EXISTS(SELECT 1 FROM dbo.intereses_futuros WITH(UPDLOCK,HOLDLOCK) WHERE docente=@OriginalDocente AND termino_clave=@OriginalTerminoClave AND activo=1) BEGIN SELECT 0; COMMIT TRANSACTION; RETURN; END;
IF @Docente IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.docente WITH (UPDLOCK,HOLDLOCK) WHERE cedula=@Docente AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
IF @TerminoClave IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.termino_clave WITH (UPDLOCK,HOLDLOCK) WHERE termino=@TerminoClave AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
UPDATE dbo.intereses_futuros SET docente=CASE WHEN @Parcial=1 THEN COALESCE(@Docente,docente) ELSE @Docente END,
 termino_clave=CASE WHEN @Parcial=1 THEN COALESCE(@TerminoClave,termino_clave) ELSE @TerminoClave END WHERE docente=@OriginalDocente AND termino_clave=@OriginalTerminoClave AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_intereses_futuros
 @Docente INT,
 @TerminoClave VARCHAR(30)
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;

UPDATE dbo.intereses_futuros SET activo=0 WHERE docente=@Docente AND termino_clave=@TerminoClave AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_listar_evaluacion_docente
 @Limite INT = 1000, @Maestro INT = NULL
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT TOP (@Limite) id AS Id, calificacion AS Calificacion, semestre AS Semestre, docente AS Docente FROM dbo.evaluacion_docente WHERE activo=1 AND (@Maestro IS NULL OR docente=@Maestro) ORDER BY id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_evaluacion_docente
 @Id INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT id AS Id, calificacion AS Calificacion, semestre AS Semestre, docente AS Docente FROM dbo.evaluacion_docente WHERE id=@Id AND activo=1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_crear_evaluacion_docente
 @Calificacion REAL,
 @Semestre VARCHAR(45),
 @Docente INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF @Docente IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.docente WITH (UPDLOCK,HOLDLOCK) WHERE cedula=@Docente AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
INSERT INTO dbo.evaluacion_docente (calificacion,semestre,docente,activo) VALUES (@Calificacion,@Semestre,@Docente,1);
DECLARE @Id INT=CONVERT(INT,SCOPE_IDENTITY());
SELECT id AS Id, calificacion AS Calificacion, semestre AS Semestre, docente AS Docente FROM dbo.evaluacion_docente WHERE id=@Id AND activo=1;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_evaluacion_docente
 @OriginalId INT,
 @Calificacion REAL = NULL,
 @Semestre VARCHAR(45) = NULL,
 @Docente INT = NULL,
 @Parcial BIT=0
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF NOT EXISTS(SELECT 1 FROM dbo.evaluacion_docente WITH(UPDLOCK,HOLDLOCK) WHERE id=@OriginalId AND activo=1) BEGIN SELECT 0; COMMIT TRANSACTION; RETURN; END;
IF @Docente IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.docente WITH (UPDLOCK,HOLDLOCK) WHERE cedula=@Docente AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
UPDATE dbo.evaluacion_docente SET calificacion=CASE WHEN @Parcial=1 THEN COALESCE(@Calificacion,calificacion) ELSE @Calificacion END,
 semestre=CASE WHEN @Parcial=1 THEN COALESCE(@Semestre,semestre) ELSE @Semestre END,
 docente=CASE WHEN @Parcial=1 THEN COALESCE(@Docente,docente) ELSE @Docente END WHERE id=@OriginalId AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_evaluacion_docente
 @Id INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;

UPDATE dbo.evaluacion_docente SET activo=0 WHERE id=@Id AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_listar_reconocimiento
 @Limite INT = 1000, @Maestro INT = NULL
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT TOP (@Limite) id AS Id, tipo AS Tipo, fecha AS Fecha, institucion AS Institucion, nombre AS Nombre, ambito AS Ambito, docente AS Docente FROM dbo.reconocimiento WHERE activo=1 AND (@Maestro IS NULL OR docente=@Maestro) ORDER BY id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_reconocimiento
 @Id INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT id AS Id, tipo AS Tipo, fecha AS Fecha, institucion AS Institucion, nombre AS Nombre, ambito AS Ambito, docente AS Docente FROM dbo.reconocimiento WHERE id=@Id AND activo=1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_crear_reconocimiento
 @Tipo VARCHAR(45),
 @Fecha DATE,
 @Institucion VARCHAR(45),
 @Nombre VARCHAR(45),
 @Ambito VARCHAR(45),
 @Docente INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF @Docente IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.docente WITH (UPDLOCK,HOLDLOCK) WHERE cedula=@Docente AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
INSERT INTO dbo.reconocimiento (tipo,fecha,institucion,nombre,ambito,docente,activo) VALUES (@Tipo,@Fecha,@Institucion,@Nombre,@Ambito,@Docente,1);
DECLARE @Id INT=CONVERT(INT,SCOPE_IDENTITY());
SELECT id AS Id, tipo AS Tipo, fecha AS Fecha, institucion AS Institucion, nombre AS Nombre, ambito AS Ambito, docente AS Docente FROM dbo.reconocimiento WHERE id=@Id AND activo=1;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_reconocimiento
 @OriginalId INT,
 @Tipo VARCHAR(45) = NULL,
 @Fecha DATE = NULL,
 @Institucion VARCHAR(45) = NULL,
 @Nombre VARCHAR(45) = NULL,
 @Ambito VARCHAR(45) = NULL,
 @Docente INT = NULL,
 @Parcial BIT=0
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF NOT EXISTS(SELECT 1 FROM dbo.reconocimiento WITH(UPDLOCK,HOLDLOCK) WHERE id=@OriginalId AND activo=1) BEGIN SELECT 0; COMMIT TRANSACTION; RETURN; END;
IF @Docente IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.docente WITH (UPDLOCK,HOLDLOCK) WHERE cedula=@Docente AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
UPDATE dbo.reconocimiento SET tipo=CASE WHEN @Parcial=1 THEN COALESCE(@Tipo,tipo) ELSE @Tipo END,
 fecha=CASE WHEN @Parcial=1 THEN COALESCE(@Fecha,fecha) ELSE @Fecha END,
 institucion=CASE WHEN @Parcial=1 THEN COALESCE(@Institucion,institucion) ELSE @Institucion END,
 nombre=CASE WHEN @Parcial=1 THEN COALESCE(@Nombre,nombre) ELSE @Nombre END,
 ambito=CASE WHEN @Parcial=1 THEN COALESCE(@Ambito,ambito) ELSE @Ambito END,
 docente=CASE WHEN @Parcial=1 THEN COALESCE(@Docente,docente) ELSE @Docente END WHERE id=@OriginalId AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_reconocimiento
 @Id INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;

UPDATE dbo.reconocimiento SET activo=0 WHERE id=@Id AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_listar_experiecia
 @Limite INT = 1000, @Maestro INT = NULL
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT TOP (@Limite) id AS Id, nombre_cargo AS NombreCargo, institucion AS Institucion, tipo AS Tipo, fecha_inicio AS FechaInicio, fecha_fin AS FechaFin, docente AS Docente FROM dbo.experiecia WHERE activo=1 AND (@Maestro IS NULL OR docente=@Maestro) ORDER BY id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_experiecia
 @Id INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT id AS Id, nombre_cargo AS NombreCargo, institucion AS Institucion, tipo AS Tipo, fecha_inicio AS FechaInicio, fecha_fin AS FechaFin, docente AS Docente FROM dbo.experiecia WHERE id=@Id AND activo=1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_crear_experiecia
 @NombreCargo VARCHAR(45),
 @Institucion VARCHAR(45),
 @Tipo VARCHAR(45),
 @FechaInicio DATE,
 @FechaFin DATE,
 @Docente INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF @Docente IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.docente WITH (UPDLOCK,HOLDLOCK) WHERE cedula=@Docente AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
INSERT INTO dbo.experiecia (nombre_cargo,institucion,tipo,fecha_inicio,fecha_fin,docente,activo) VALUES (@NombreCargo,@Institucion,@Tipo,@FechaInicio,@FechaFin,@Docente,1);
DECLARE @Id INT=CONVERT(INT,SCOPE_IDENTITY());
SELECT id AS Id, nombre_cargo AS NombreCargo, institucion AS Institucion, tipo AS Tipo, fecha_inicio AS FechaInicio, fecha_fin AS FechaFin, docente AS Docente FROM dbo.experiecia WHERE id=@Id AND activo=1;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_experiecia
 @OriginalId INT,
 @NombreCargo VARCHAR(45) = NULL,
 @Institucion VARCHAR(45) = NULL,
 @Tipo VARCHAR(45) = NULL,
 @FechaInicio DATE = NULL,
 @FechaFin DATE = NULL,
 @Docente INT = NULL,
 @Parcial BIT=0
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF NOT EXISTS(SELECT 1 FROM dbo.experiecia WITH(UPDLOCK,HOLDLOCK) WHERE id=@OriginalId AND activo=1) BEGIN SELECT 0; COMMIT TRANSACTION; RETURN; END;
IF @Docente IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.docente WITH (UPDLOCK,HOLDLOCK) WHERE cedula=@Docente AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
UPDATE dbo.experiecia SET nombre_cargo=CASE WHEN @Parcial=1 THEN COALESCE(@NombreCargo,nombre_cargo) ELSE @NombreCargo END,
 institucion=CASE WHEN @Parcial=1 THEN COALESCE(@Institucion,institucion) ELSE @Institucion END,
 tipo=CASE WHEN @Parcial=1 THEN COALESCE(@Tipo,tipo) ELSE @Tipo END,
 fecha_inicio=CASE WHEN @Parcial=1 THEN COALESCE(@FechaInicio,fecha_inicio) ELSE @FechaInicio END,
 fecha_fin=CASE WHEN @Parcial=1 THEN COALESCE(@FechaFin,fecha_fin) ELSE @FechaFin END,
 docente=CASE WHEN @Parcial=1 THEN COALESCE(@Docente,docente) ELSE @Docente END WHERE id=@OriginalId AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_experiecia
 @Id INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;

UPDATE dbo.experiecia SET activo=0 WHERE id=@Id AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_listar_red_docente
 @Limite INT = 1000, @Maestro INT = NULL
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT TOP (@Limite) red AS Red, docente AS Docente, fecha_inicio AS FechaInicio, fecha_fin AS FechaFin, act_destacadas AS ActDestacadas FROM dbo.red_docente WHERE activo=1 AND (@Maestro IS NULL OR docente=@Maestro) ORDER BY red,docente;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_red_docente
 @Red INT,
 @Docente INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT red AS Red, docente AS Docente, fecha_inicio AS FechaInicio, fecha_fin AS FechaFin, act_destacadas AS ActDestacadas FROM dbo.red_docente WHERE red=@Red AND docente=@Docente AND activo=1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_crear_red_docente
 @Red INT,
 @Docente INT,
 @FechaInicio DATE,
 @FechaFin VARCHAR(45),
 @ActDestacadas VARCHAR(MAX)
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF @Red IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.red WITH (UPDLOCK,HOLDLOCK) WHERE idr=@Red AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
IF @Docente IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.docente WITH (UPDLOCK,HOLDLOCK) WHERE cedula=@Docente AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
INSERT INTO dbo.red_docente (red,docente,fecha_inicio,fecha_fin,act_destacadas,activo) VALUES (@Red,@Docente,@FechaInicio,@FechaFin,@ActDestacadas,1);
SELECT red AS Red, docente AS Docente, fecha_inicio AS FechaInicio, fecha_fin AS FechaFin, act_destacadas AS ActDestacadas FROM dbo.red_docente WHERE red=@Red AND docente=@Docente AND activo=1;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_red_docente
 @OriginalRed INT,
 @OriginalDocente INT,
 @Red INT = NULL,
 @Docente INT = NULL,
 @FechaInicio DATE = NULL,
 @FechaFin VARCHAR(45) = NULL,
 @ActDestacadas VARCHAR(MAX) = NULL,
 @Parcial BIT=0
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF NOT EXISTS(SELECT 1 FROM dbo.red_docente WITH(UPDLOCK,HOLDLOCK) WHERE red=@OriginalRed AND docente=@OriginalDocente AND activo=1) BEGIN SELECT 0; COMMIT TRANSACTION; RETURN; END;
IF @Red IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.red WITH (UPDLOCK,HOLDLOCK) WHERE idr=@Red AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
IF @Docente IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.docente WITH (UPDLOCK,HOLDLOCK) WHERE cedula=@Docente AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
UPDATE dbo.red_docente SET red=CASE WHEN @Parcial=1 THEN COALESCE(@Red,red) ELSE @Red END,
 docente=CASE WHEN @Parcial=1 THEN COALESCE(@Docente,docente) ELSE @Docente END,
 fecha_inicio=CASE WHEN @Parcial=1 THEN COALESCE(@FechaInicio,fecha_inicio) ELSE @FechaInicio END,
 fecha_fin=CASE WHEN @Parcial=1 THEN COALESCE(@FechaFin,fecha_fin) ELSE @FechaFin END,
 act_destacadas=CASE WHEN @Parcial=1 THEN COALESCE(@ActDestacadas,act_destacadas) ELSE @ActDestacadas END WHERE red=@OriginalRed AND docente=@OriginalDocente AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_red_docente
 @Red INT,
 @Docente INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;

UPDATE dbo.red_docente SET activo=0 WHERE red=@Red AND docente=@Docente AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_listar_estudio_ac
 @Limite INT = 1000, @Maestro INT = NULL
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT TOP (@Limite) estudio AS Estudio, area_conocimiento AS AreaConocimiento FROM dbo.estudio_ac WHERE activo=1 AND (@Maestro IS NULL OR estudio=@Maestro) ORDER BY estudio,area_conocimiento;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_estudio_ac
 @Estudio INT,
 @AreaConocimiento VARCHAR(6)
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT estudio AS Estudio, area_conocimiento AS AreaConocimiento FROM dbo.estudio_ac WHERE estudio=@Estudio AND area_conocimiento=@AreaConocimiento AND activo=1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_crear_estudio_ac
 @Estudio INT,
 @AreaConocimiento VARCHAR(6)
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF @Estudio IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.estudios_realizados WITH (UPDLOCK,HOLDLOCK) WHERE id=@Estudio AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
IF @AreaConocimiento IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.area_conocimiento WITH (UPDLOCK,HOLDLOCK) WHERE id=@AreaConocimiento AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
INSERT INTO dbo.estudio_ac (estudio,area_conocimiento,activo) VALUES (@Estudio,@AreaConocimiento,1);
SELECT estudio AS Estudio, area_conocimiento AS AreaConocimiento FROM dbo.estudio_ac WHERE estudio=@Estudio AND area_conocimiento=@AreaConocimiento AND activo=1;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_estudio_ac
 @OriginalEstudio INT,
 @OriginalAreaConocimiento VARCHAR(6),
 @Estudio INT = NULL,
 @AreaConocimiento VARCHAR(6) = NULL,
 @Parcial BIT=0
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF NOT EXISTS(SELECT 1 FROM dbo.estudio_ac WITH(UPDLOCK,HOLDLOCK) WHERE estudio=@OriginalEstudio AND area_conocimiento=@OriginalAreaConocimiento AND activo=1) BEGIN SELECT 0; COMMIT TRANSACTION; RETURN; END;
IF @Estudio IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.estudios_realizados WITH (UPDLOCK,HOLDLOCK) WHERE id=@Estudio AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
IF @AreaConocimiento IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.area_conocimiento WITH (UPDLOCK,HOLDLOCK) WHERE id=@AreaConocimiento AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
UPDATE dbo.estudio_ac SET estudio=CASE WHEN @Parcial=1 THEN COALESCE(@Estudio,estudio) ELSE @Estudio END,
 area_conocimiento=CASE WHEN @Parcial=1 THEN COALESCE(@AreaConocimiento,area_conocimiento) ELSE @AreaConocimiento END WHERE estudio=@OriginalEstudio AND area_conocimiento=@OriginalAreaConocimiento AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_estudio_ac
 @Estudio INT,
 @AreaConocimiento VARCHAR(6)
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;

UPDATE dbo.estudio_ac SET activo=0 WHERE estudio=@Estudio AND area_conocimiento=@AreaConocimiento AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_listar_apoyo_profesoral
 @Limite INT = 1000, @Maestro INT = NULL
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT TOP (@Limite) estudios AS Estudios, con_apoyo AS ConApoyo, institucion AS Institucion, tipo AS Tipo FROM dbo.apoyo_profesoral WHERE activo=1 AND (@Maestro IS NULL OR estudios=@Maestro) ORDER BY estudios;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_apoyo_profesoral
 @Estudios INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT estudios AS Estudios, con_apoyo AS ConApoyo, institucion AS Institucion, tipo AS Tipo FROM dbo.apoyo_profesoral WHERE estudios=@Estudios AND activo=1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_crear_apoyo_profesoral
 @Estudios INT,
 @ConApoyo TINYINT,
 @Institucion VARCHAR(45),
 @Tipo VARCHAR(45)
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF @Estudios IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.estudios_realizados WITH (UPDLOCK,HOLDLOCK) WHERE id=@Estudios AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
INSERT INTO dbo.apoyo_profesoral (estudios,con_apoyo,institucion,tipo,activo) VALUES (@Estudios,@ConApoyo,@Institucion,@Tipo,1);
SELECT estudios AS Estudios, con_apoyo AS ConApoyo, institucion AS Institucion, tipo AS Tipo FROM dbo.apoyo_profesoral WHERE estudios=@Estudios AND activo=1;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_apoyo_profesoral
 @OriginalEstudios INT,
 @Estudios INT = NULL,
 @ConApoyo TINYINT = NULL,
 @Institucion VARCHAR(45) = NULL,
 @Tipo VARCHAR(45) = NULL,
 @Parcial BIT=0
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF NOT EXISTS(SELECT 1 FROM dbo.apoyo_profesoral WITH(UPDLOCK,HOLDLOCK) WHERE estudios=@OriginalEstudios AND activo=1) BEGIN SELECT 0; COMMIT TRANSACTION; RETURN; END;
IF @Estudios IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.estudios_realizados WITH (UPDLOCK,HOLDLOCK) WHERE id=@Estudios AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
UPDATE dbo.apoyo_profesoral SET estudios=CASE WHEN @Parcial=1 THEN COALESCE(@Estudios,estudios) ELSE @Estudios END,
 con_apoyo=CASE WHEN @Parcial=1 THEN COALESCE(@ConApoyo,con_apoyo) ELSE @ConApoyo END,
 institucion=CASE WHEN @Parcial=1 THEN COALESCE(@Institucion,institucion) ELSE @Institucion END,
 tipo=CASE WHEN @Parcial=1 THEN COALESCE(@Tipo,tipo) ELSE @Tipo END WHERE estudios=@OriginalEstudios AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_apoyo_profesoral
 @Estudios INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;

UPDATE dbo.apoyo_profesoral SET activo=0 WHERE estudios=@Estudios AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_listar_beca
 @Limite INT = 1000, @Maestro INT = NULL
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT TOP (@Limite) estudios AS Estudios, tipo AS Tipo, institucion AS Institucion, fecha_inicio AS FechaInicio, fecha_fin AS FechaFin FROM dbo.beca WHERE activo=1 AND (@Maestro IS NULL OR estudios=@Maestro) ORDER BY estudios;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_beca
 @Estudios INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
SELECT estudios AS Estudios, tipo AS Tipo, institucion AS Institucion, fecha_inicio AS FechaInicio, fecha_fin AS FechaFin FROM dbo.beca WHERE estudios=@Estudios AND activo=1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_crear_beca
 @Estudios INT,
 @Tipo VARCHAR(45),
 @Institucion VARCHAR(80),
 @FechaInicio DATE,
 @FechaFin DATE
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF @Estudios IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.estudios_realizados WITH (UPDLOCK,HOLDLOCK) WHERE id=@Estudios AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
INSERT INTO dbo.beca (estudios,tipo,institucion,fecha_inicio,fecha_fin,activo) VALUES (@Estudios,@Tipo,@Institucion,@FechaInicio,@FechaFin,1);
SELECT estudios AS Estudios, tipo AS Tipo, institucion AS Institucion, fecha_inicio AS FechaInicio, fecha_fin AS FechaFin FROM dbo.beca WHERE estudios=@Estudios AND activo=1;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_beca
 @OriginalEstudios INT,
 @Estudios INT = NULL,
 @Tipo VARCHAR(45) = NULL,
 @Institucion VARCHAR(80) = NULL,
 @FechaInicio DATE = NULL,
 @FechaFin DATE = NULL,
 @Parcial BIT=0
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;
IF NOT EXISTS(SELECT 1 FROM dbo.beca WITH(UPDLOCK,HOLDLOCK) WHERE estudios=@OriginalEstudios AND activo=1) BEGIN SELECT 0; COMMIT TRANSACTION; RETURN; END;
IF @Estudios IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.estudios_realizados WITH (UPDLOCK,HOLDLOCK) WHERE id=@Estudios AND activo=1) THROW 51001, N'La relación seleccionada no existe o está retirada.', 1;
UPDATE dbo.beca SET estudios=CASE WHEN @Parcial=1 THEN COALESCE(@Estudios,estudios) ELSE @Estudios END,
 tipo=CASE WHEN @Parcial=1 THEN COALESCE(@Tipo,tipo) ELSE @Tipo END,
 institucion=CASE WHEN @Parcial=1 THEN COALESCE(@Institucion,institucion) ELSE @Institucion END,
 fecha_inicio=CASE WHEN @Parcial=1 THEN COALESCE(@FechaInicio,fecha_inicio) ELSE @FechaInicio END,
 fecha_fin=CASE WHEN @Parcial=1 THEN COALESCE(@FechaFin,fecha_fin) ELSE @FechaFin END WHERE estudios=@OriginalEstudios AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_beca
 @Estudios INT
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRY
 BEGIN TRANSACTION;

UPDATE dbo.beca SET activo=0 WHERE estudios=@Estudios AND activo=1;
SELECT @@ROWCOUNT;
 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
 THROW;
 END CATCH
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_consultar_retiro_area_conocimiento
 @Id VARCHAR(6)
AS BEGIN SET NOCOUNT ON; SELECT CAST(CASE WHEN EXISTS(SELECT 1 FROM dbo.estudio_ac WHERE area_conocimiento=@Id AND activo=1) THEN 1 ELSE 0 END AS BIT); END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_retiro_termino_clave
 @Termino VARCHAR(30)
AS BEGIN SET NOCOUNT ON; SELECT CAST(CASE WHEN EXISTS(SELECT 1 FROM dbo.intereses_futuros WHERE termino_clave=@Termino AND activo=1) THEN 1 ELSE 0 END AS BIT); END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_retiro_linea_investigacion
 @Id INT
AS BEGIN SET NOCOUNT ON; SELECT CAST(CASE WHEN EXISTS(SELECT 1 FROM dbo.docente WHERE linea_investigacion_principal=@Id AND activo=1) THEN 1 ELSE 0 END AS BIT); END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_retiro_programa
 @Id INT
AS BEGIN SET NOCOUNT ON; SELECT CAST(CASE WHEN EXISTS(SELECT 1 FROM dbo.docente_departamento WHERE departamento=@Id AND activo=1) THEN 1 ELSE 0 END AS BIT); END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_retiro_red
 @Idr INT
AS BEGIN SET NOCOUNT ON; SELECT CAST(CASE WHEN EXISTS(SELECT 1 FROM dbo.red_docente WHERE red=@Idr AND activo=1) THEN 1 ELSE 0 END AS BIT); END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_retiro_docente
 @Cedula INT
AS BEGIN SET NOCOUNT ON; SELECT CAST(CASE WHEN EXISTS(SELECT 1 FROM dbo.estudios_realizados WHERE docente=@Cedula AND activo=1) OR EXISTS(SELECT 1 FROM dbo.docente_departamento WHERE docente=@Cedula AND activo=1) OR EXISTS(SELECT 1 FROM dbo.intereses_futuros WHERE docente=@Cedula AND activo=1) OR EXISTS(SELECT 1 FROM dbo.evaluacion_docente WHERE docente=@Cedula AND activo=1) OR EXISTS(SELECT 1 FROM dbo.reconocimiento WHERE docente=@Cedula AND activo=1) OR EXISTS(SELECT 1 FROM dbo.experiecia WHERE docente=@Cedula AND activo=1) OR EXISTS(SELECT 1 FROM dbo.red_docente WHERE docente=@Cedula AND activo=1) THEN 1 ELSE 0 END AS BIT); END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_retiro_estudios_realizados
 @Id INT
AS BEGIN SET NOCOUNT ON; SELECT CAST(CASE WHEN EXISTS(SELECT 1 FROM dbo.estudio_ac WHERE estudio=@Id AND activo=1) OR EXISTS(SELECT 1 FROM dbo.apoyo_profesoral WHERE estudios=@Id AND activo=1) OR EXISTS(SELECT 1 FROM dbo.beca WHERE estudios=@Id AND activo=1) THEN 1 ELSE 0 END AS BIT); END;
GO
